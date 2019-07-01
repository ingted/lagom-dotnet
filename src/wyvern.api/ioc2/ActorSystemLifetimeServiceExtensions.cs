using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Akka;
using Akka.Actor;
using Microsoft.AspNetCore.Builder;
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
using static ReactiveServiceRouteBuilder;
using static wyvern.api.ioc.ServiceExtensions;

public static class ReactivceServicesServiceExtensions
{

    public static IApplicationBuilder ActivateServiceRegistry(this IApplicationBuilder app)
    {
        var registry = app.ApplicationServices.GetService<IShardedEntityRegistry>();
        var entities = ReactiveReflector.GetEntityTypes();
        foreach (var entity in entities)
        {
            var registerMethod = typeof(ShardedEntityRegistry).GetMethod("Register");
            var types = entity.BaseType.GenericTypeArguments;
            var generic = registerMethod.MakeGenericMethod(
                entity, types[0], types[1], types[2]
            );
            var factory = typeof(InstanceFactory)
                .GetMethod("CreateInstance")
                .MakeGenericMethod(entity, entity)
                .Invoke(null, null);
            generic.Invoke(registry, new object[] { factory });
        }
        return app;
    }

    // ! TODO: Streams
    // public static IApplicationBuilder UseReactiveServicesStreams(this IApplicationBuilder app)
    // {
    //     // app.Use(async (context, next) =>
    //     // {
    //     // // TODO: this isn't matching with embedded path variables.
    //     // if (context.Request.Path != path)
    //     //     {
    //     //         await next();
    //     //     }
    //     //     else
    //     //     {
    //     //         if (!context.WebSockets.IsWebSocketRequest)
    //     //             return;

    //     //         object[] mrefParamArray = mrefParams.Select(x =>
    //     //             {
    //     //                 var type = x.ParameterType;
    //     //                 var name = x.Name;
    //     //                 try
    //     //                 {
    //     //                     var data = context.Request.Query;
    //     //                     var val = data[name].ToString();
    //     //                     if (type == typeof(String))
    //     //                         return val as object;
    //     //                     if (type == typeof(Int64))
    //     //                         return Int64.Parse(val) as object;
    //     //                     if (type == typeof(Int32))
    //     //                         return Int32.Parse(val) as object;
    //     //                     if (type == typeof(Int16))
    //     //                         return Int16.Parse(val) as object;

    //     //                     throw new Exception("Unsupported path parameter type: " + type.Name);
    //     //                 }
    //     //                 catch (Exception)
    //     //                 {
    //     //                     throw new Exception($"Failed to match URL parameter [{name}] in path template.");
    //     //                 }
    //     //             })
    //     //             .ToArray();

    //     //         var socket = await context.WebSockets.AcceptWebSocketAsync();

    //     //         var service = serviceFac();
    //     //         var mres = mref.Invoke(service, mrefParamArray);
    //     //         var cref = mres.GetType().GetMethod("Invoke");
    //     //         var t = (Task)cref.Invoke(mres, new object[] { socket });
    //     //         await t.ConfigureAwait(false);
    //     //     }
    //     // });
    // }

    public static IApplicationBuilder UseReactiveServicesRouter(this IApplicationBuilder app)
    {
        var apiContractResolver = app.ApplicationServices.GetService<IContractResolver>();
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = apiContractResolver ?? new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        var localServiceTypes = ReactiveReflector.GetLocalServiceTypes();
        Dictionary<Type, Service> serviceLookup = new Dictionary<Type, Service>();
        foreach (var localServiceType in localServiceTypes)
            serviceLookup.Add(localServiceType, (Service)app.ApplicationServices.GetService(localServiceType));

        var appRouter = new RouteBuilder(app);

        foreach (var service in serviceLookup)
        {
            var type = service.Key;
            foreach (var call in service.Value.Descriptor.Calls.Where(x => x.CallId as RestCallId != null))
            {
                var callRouter = ReactiveServiceRouteBuilder.ExtractRoutePath(call);
                var routeParams = ServiceCallRouteParameters.Parse(call);

                callRouter.Map(appRouter)
                    .Invoke(
                        callRouter.PathPattern,
                        async (req, res, data) =>
                        {
                            object[] mrefParamArray = routeParams.CreateParameterArray(data);

                            var serverServiceCall = routeParams.ServiceRoute.DynamicInvoke(mrefParamArray);
                            var handleRequestHeader = serverServiceCall.GetType().GetMethod("HandleRequestHeader");
                            var filter = new Func<RequestHeader, RequestHeader>(header =>
                            {
                                foreach (var (k, v) in req.Headers)
                                    header = header.WithHeader(k, v);
                                return service.Value.Descriptor.HeaderFilter.TransformServerRequest(header);
                            });
                            var serviceCall = handleRequestHeader.Invoke(serverServiceCall, new object[] { filter });
                            var serviceCallInvoke = serverServiceCall.GetType().GetMethod("Invoke");

                            // TODO: translate the header response
                            // Note, header response should have HTTP Status Code embedded
                            // Better handling for the exceptions and exception mapping...

                            dynamic task;
                            if (routeParams.RequestType == typeof(NotUsed))
                            {
                                task = serviceCallInvoke.Invoke(serviceCall, new object[] { NotUsed.Instance });
                            }
                            else
                            {
                                string body;
                                using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                                    body = reader.ReadToEnd();

                                var obj = JsonConvert.DeserializeObject(body, routeParams.RequestType, serializerSettings);
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

            }
        }

        var routes = appRouter.Build();

        // Support DI for unit tests
        var consumer = app.ApplicationServices.GetService<IRouteCollectionConsumer>();
        consumer?.Consume(routes as RouteCollection);

        app.UseRouter(routes);

        var config = app.ApplicationServices.GetService<IConfiguration>();
        var swaggerDocsApiName = config.GetValue<string>("SwaggerDocs:ApiName", "My API V1");

        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerDocsApiName);
            x.RoutePrefix = string.Empty;
        });

        return app;
    }

    public static IServiceCollection ConfigureReactiveServicesApi(this IServiceCollection services)
    {
        foreach (var serviceType in ReactiveReflector.GetLocalServiceTypes())
            services.AddTransient(serviceType);
        services.AddCors();
        services.AddSingleton<IContractResolver>(
            x => new CamelCasePropertyNamesContractResolver()
        );
        services.AddRouting();
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
        return services;
    }

    public static IServiceCollection AddActorSystem(this IServiceCollection services)
    {
        services.AddSingleton<ConfigurationLoader>();

        services.AddSingleton<ActorSystemLifetime>();
        services.AddSingleton<ActorSystem>(x =>
            x.GetService<ActorSystemLifetime>()
                .CreateActorSystem()
        );

        services.AddTransient<ISerializer>(y => new DefaultSerializer());
        services.AddTransient<IMessagePropertyExtractor>(y => new DefaultExtractor());

        return services;
    }

    public static IServiceCollection AddReactiveComponents(this IServiceCollection services)
    {
        services.AddSingleton<IShardedEntityRegistry, ShardedEntityRegistry>();

        // Register all local services
        var serviceTypes = ReactiveReflector.GetLocalServiceTypes();
        foreach (var serviceType in serviceTypes)
            services.AddTransient(serviceType.BaseType, serviceType);

        // TODO: Any readside processors

        // var remoteServiceTypes = ReactiveReflector.GetRemoteServiceTypes();
        //foreach (var serviceType in remoteServiceTypes)
        // services.AddTransient(/* TODO: Service resolver */)

        // TODO: Topics

        return services;
    }
}
