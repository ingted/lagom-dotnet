// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.IO;
using System.Text;
using Akka;
using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using wyvern.api;
using wyvern.api.abstractions;
using wyvern.api.exceptions;
using wyvern.api.@internal.sharding;
using wyvern.api.@internal.surfaces;
using wyvern.api.ioc;
using wyvern.utils;

public class ReactiveServicesStartup
{
    public ReactiveServicesStartup()
    {

    }

    /// <summary>
    /// Add services
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        // TODO: This isn't as nice as I would like it to be, revisit soon...
        services.AddCors()
                .AddRouting()
                .AddSingleton<ConfigurationLoader>()
                .AddSingleton<ActorSystemLifetime>()
                .AddSingleton<ActorSystem>(x => {
                    var actorSystem = x.GetService<ActorSystemLifetime>().CreateActorSystem();
                    return actorSystem;
                });

        services.AddTransient<IShardedEntityRegistry, ShardedEntityRegistry>();
        services.AddTransient<ISerializer, DefaultSerializer>();

        foreach (var serviceType in ReactiveReflector.GetLocalServiceTypes())
        {
            Console.WriteLine(serviceType);
            services.AddTransient(serviceType);
        }

        services.TryAddSingleton<IApiDescriptionGroupCollectionProvider, ReactiveServicesApiDescriptionGroupProvider>();
        services.AddSwaggerGen(c =>
        {
            c.DocumentFilter<ReactiveServicesApiDescriptionsDocumentFilter>();
            c.SwaggerDoc("v1", new Info()
            {
                Title = "My Reactive Services",
                Version = "v1"
            });
        });

    }

    /// <summary>
    /// Configure the HTTP pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration config)
    {
        // ! TODO: Should be configurable
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseWebSockets();

         var router = new RouteBuilder(app);

        // Register all calls for the services
        var entityTypes = ReactiveReflector.GetEntityTypes();

        var serviceTypes = ReactiveReflector.GetLocalServiceTypes();
        var apiContractResolver = app.ApplicationServices.GetService<IContractResolver>();
        foreach (var serviceType in serviceTypes)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = apiContractResolver ?? new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            var service = (Service)app.ApplicationServices.GetService(serviceType);
            foreach (var call in service.Descriptor.Calls)
            {
                switch (call.CallId)
                {
                    case RestCallId restCall:
                        var serviceCallRouter = ReactiveServiceRouteBuilder
                            .ExtractServiceCallRouter(call);
                        var serviceCallRouteParameters = ReactiveServiceRouteBuilder.ServiceCallRouteParameters.Parse(call);
                        serviceCallRouter.Map(router)
                        .Invoke(
                            serviceCallRouter.PathPattern,
                            async (req, res, data) =>
                            {
                                var mrefParamArray = serviceCallRouteParameters.CreateParameterArray(data);
                                var serverServiceCall = serviceCallRouteParameters
                                    .ServiceRoute.DynamicInvoke(mrefParamArray);
                                var handleRequestHeader = serverServiceCall
                                    .GetType().GetMethod("HandleRequestHeader");

                                var filter = new Func<RequestHeader, RequestHeader>(header =>
                                {
                                    foreach (var (k, v) in req.Headers)
                                        header = header.WithHeader(k, v);
                                    return service.Descriptor.HeaderFilter.TransformServerRequest(header);
                                });

                                var serviceCall = handleRequestHeader.Invoke(serverServiceCall, new object[] { filter });
                                var serviceCallInvoke = serverServiceCall.GetType().GetMethod("Invoke");

                                // TODO: translate the header response
                                // Note, header response should have HTTP Status Code embedded
                                // Better handling for the exceptions and exception mapping...

                                dynamic task;
                                if (serviceCallRouteParameters.RequestType == typeof(NotUsed))
                                {
                                    task = serviceCallInvoke.Invoke(serviceCall, new object[] { NotUsed.Instance });
                                }
                                else
                                {
                                    string body;
                                    using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                                        body = reader.ReadToEnd();

                                    var obj = JsonConvert.DeserializeObject(body, serviceCallRouteParameters.RequestType, serializerSettings);
                                    task = serviceCallInvoke.Invoke(serviceCall, new object[] { obj });
                                }

                                try
                                {
                                    await task;
                                    if (task.Result is Exception)
                                        throw task.Result as Exception;
                                }
                                catch (Exception ex)
                                {
                                    if (ex is StatusCodeException) throw;
                                    // TODO: Add cors headers to allow repsonse
                                    res.StatusCode = 500;
                                    var result = task.Result as Exception;
                                    var jsonString = JsonConvert.SerializeObject(result.Message, serializerSettings);
                                    byte[] content = Encoding.UTF8.GetBytes(jsonString);
                                    res.ContentType = "application/json";
                                    await res.Body.WriteAsync(content, 0, content.Length);
                                    return;
                                }

                                {
                                    var result = task.Result;
                                    var jsonString = JsonConvert.SerializeObject(result, serializerSettings);
                                    byte[] content = Encoding.UTF8.GetBytes(jsonString);
                                    res.ContentType = "application/json";
                                    await res.Body.WriteAsync(content, 0, content.Length);
                                }
                            }
                        );

                        break;
                    default:
                        break;
                        //throw new Exception("Unknown call type");
                }

            }
        }

        // TODO: Visualizer

        // Build the API
        var routes = router.Build();
        // var consumer = services.GetService<IRouteCollectionConsumer>();
        // consumer?.Consume(routes as RouteCollection);
        app.UseRouter(routes);

        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                config.GetValue<string>("SwaggerDocs:ApiName", "My API V1")
            );
            x.RoutePrefix = string.Empty;
        });



        // TODO: register streams
        // TODO: services by reflection
        // TODO: register remote services.
    }
}
