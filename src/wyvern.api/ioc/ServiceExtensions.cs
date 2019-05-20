using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using wyvern.api.abstractions;
using wyvern.api.exceptions;
using wyvern.api.@internal.surfaces;
using wyvern.utils;
using wyvern.visualize;

namespace wyvern.api.ioc
{
    public static partial class ServiceExtensions
    {
        public interface IRouteCollectionConsumer
        {
            void Consume(RouteCollection collection);
        }

        [Obsolete("WARNING: need to remove this for unit tests to work properly")]
        static ReactiveServicesOption Options;

        /// <summary>
        /// Add the main reactive services components, including swagger generation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builderDelegate"></param>
        /// <returns></returns>
        public static IServiceCollection AddReactiveServices(this IServiceCollection services,
            Action<IReactiveServicesBuilder> builderDelegate, ReactiveServicesOption options)
        {
            Options = options;

            // Add reactive services core
            var builder = new ReactiveServicesBuilder();
            builderDelegate(builder);
            services.AddSingleton(builder.Build(services));

            // Optionally, expose reactive services via API
            if (Options.HasFlag(ReactiveServicesOption.WithApi))
            {
                // Routing for mapping HTTP URLs in Kestrel
                services.AddRouting();

                // Swagger generation
                if (Options.HasFlag(ReactiveServicesOption.WithSwagger))
                {
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
            }

            return services;
        }

        public static IApplicationBuilder UseReactiveServices(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var reactiveServices = services.GetService<IReactiveServices>();
            var apiContractResolver = services.GetService<IContractResolver>();

            void ServiceIterator(Action<Service, Type> x)
            {
                foreach (var (serviceType, _) in reactiveServices)
                    x((Service)services.GetService(serviceType), serviceType);
            }

            // Register any service bound topics
            if (Options.HasFlag(ReactiveServicesOption.WithTopics))
            {
                ServiceIterator((service, serviceType) =>
                {
                    foreach (var topic in service.Descriptor.Topics)
                        RegisterTopic(
                            topic,
                            service,
                            services.GetService<ISerializer>(),
                            services.GetService<IMessagePropertyExtractor>(),
                            app.ApplicationServices.GetService<ActorSystem>()
                        );
                });
            }

            app.UseWebSockets();

            // Build the API components
            if (Options.HasFlag(ReactiveServicesOption.WithApi))
            {
                var router = new RouteBuilder(app);

                // Register all calls for the services
                ServiceIterator((service, serviceType) =>
                {
                    foreach (var call in service.Descriptor.Calls)
                    {
                        switch (call.CallId)
                        {
                            case RestCallId _:
                                RegisterCall(router, service, call, apiContractResolver);
                                break;
                            case StreamCallId _:
                                RegisterStream(router, service, serviceType, call, app);
                                break;
                            default:
                                throw new Exception("Unknown call type");
                        }

                    }
                });

                // Visualization components
                if (Options.HasFlag(ReactiveServicesOption.WithVisualizer))
                    AddVisualizer(router);

                // Build the API
                var routes = router.Build();
                var consumer = services.GetService<IRouteCollectionConsumer>();
                consumer?.Consume(routes as RouteCollection);
                app.UseRouter(routes);

                // Optionally, add swagger components
                if (Options.HasFlag(ReactiveServicesOption.WithSwagger))
                {
                    var config = services.GetService<IConfiguration>();
                    var swaggerDocsApiName = config.GetValue<string>("SwaggerDocs:ApiName", "My API V1");

                    app.UseSwagger();
                    app.UseSwaggerUI(x =>
                    {
                        x.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerDocsApiName);
                        x.RoutePrefix = string.Empty;
                    });
                }

            }

            return app;
        }

        /// <summary>
        /// Add a cluster visualizer to the endpoints /api/visualizer/list and
        /// /api/visualizer/send
        /// </summary>
        /// <param name="router"></param>
        private static void AddVisualizer(IRouteBuilder router)
        {
            router.MapGet("/api/visualizer/list", async (req, res, ctx) =>
            {
                req.Query.TryGetValue("path", out var path);
                var obj = await WebApiVisualizer.Root.List(path);
                var jsonString = JsonConvert.SerializeObject(obj);
                byte[] content = Encoding.UTF8.GetBytes(jsonString);
                res.ContentType = "application/json";
                await res.Body.WriteAsync(content, 0, content.Length);
            });

            router.MapGet("/api/visualizer/send", async (req, res, ctx) =>
            {
                req.Query.TryGetValue("path", out var path);
                req.Query.TryGetValue("messageType", out var messageType);
                var obj = await WebApiVisualizer.Root.Send(path, messageType);
                var jsonString = JsonConvert.SerializeObject(obj);
                byte[] content = Encoding.UTF8.GetBytes(jsonString);
                res.ContentType = "application/json";
                await res.Body.WriteAsync(content, 0, content.Length);
            });
        }

        private static void RegisterTopic(object t, Service s, ISerializer serializer, IMessagePropertyExtractor extractor, ActorSystem sys)
        {
            var topicCall = (ITopicCall)t;
            if (!(topicCall.TopicHolder is MethodTopicHolder))
                throw new NotImplementedException();

            var producer = ((MethodTopicHolder)topicCall.TopicHolder).Method.Invoke(s, null);
            typeof(ITaggedOffsetTopicProducer<>)
            .MakeGenericType(producer.GetType().GetGenericArguments()[0])
                .GetMethod("Init")
                .Invoke(producer, new object[] { sys, topicCall.TopicId.Name, serializer, extractor });

        }

        /// <summary>
        /// Register a route to the service call
        /// </summary>
        /// <param name="router"></param>
        /// <param name="service"></param>
        /// <param name="serviceType"></param>
        /// <param name="call"></param>
        private static void RegisterCall(IRouteBuilder router, Service service, ICall call, IContractResolver apiContractResolver)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = apiContractResolver ?? new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var (routeMapper, path) = ExtractRoutePath(router, call);

            var route = call.MethodRef;
            var routeMethod = call.MethodRef.Method;
            var routeParameters = routeMethod.GetParameters();

            var serviceCallType = routeMethod.ReturnType;
            var requestType = serviceCallType.GenericTypeArguments[0];

            routeMapper(
                path,
                async (req, res, data) =>
                {
                    object[] mrefParamArray = routeParameters.Select(x =>
                        {
                            var type = x.ParameterType;
                            var name = x.Name;
                            try
                            {
                                var val = data.Values[name].ToString();
                                if (type == typeof(String))
                                    return val;
                                if (type == typeof(Int64))
                                    return Int64.Parse(val) as object;
                                if (type == typeof(Int32))
                                    return Int32.Parse(val) as object;
                                if (type == typeof(Int16))
                                    return Int16.Parse(val) as object;

                                throw new Exception("Unsupported path parameter type: " + type.Name);
                            }
                            catch (Exception)
                            {
                                throw new Exception($"Failed to match URL parameter [{name}] in path template.");
                            }
                        })
                        .ToArray();

                    var serverServiceCall = route.DynamicInvoke(mrefParamArray);
                    var handleRequestHeader = serverServiceCall.GetType().GetMethod("HandleRequestHeader");

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
                    if (requestType == typeof(NotUsed))
                    {
                        task = serviceCallInvoke.Invoke(serviceCall, new object[] { NotUsed.Instance });
                    }
                    else
                    {
                        string body;
                        using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                            body = reader.ReadToEnd();

                        var obj = JsonConvert.DeserializeObject(body, requestType, serializerSettings);
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

        /// <summary>
        /// Register a route to the service call
        /// </summary>
        /// <param name="router"></param>
        /// <param name="service"></param>
        /// <param name="serviceType"></param>
        /// <param name="call"></param>
        private static void RegisterStream(IRouteBuilder router, Service service, Type serviceType, ICall call, IApplicationBuilder app)
        {
            var (_, path) = ExtractRoutePath(router, call);

            var mref = call.MethodRef.Method;
            var mrefParams = mref.GetParameters();
            var methodRefType = mref.ReturnType;

            app.Use(async (context, next) =>
            {
                // TODO: this isn't matching with embedded path variables.
                if (context.Request.Path != path)
                {
                    await next();
                }
                else
                {
                    if (!context.WebSockets.IsWebSocketRequest)
                        return;

                    object[] mrefParamArray = mrefParams.Select(x =>
                        {
                            var type = x.ParameterType;
                            var name = x.Name;
                            try
                            {
                                var data = context.Request.Query;
                                var val = data[name].ToString();
                                if (type == typeof(String))
                                    return val as object;
                                if (type == typeof(Int64))
                                    return Int64.Parse(val) as object;
                                if (type == typeof(Int32))
                                    return Int32.Parse(val) as object;
                                if (type == typeof(Int16))
                                    return Int16.Parse(val) as object;

                                throw new Exception("Unsupported path parameter type: " + type.Name);
                            }
                            catch (Exception)
                            {
                                throw new Exception($"Failed to match URL parameter [{name}] in path template.");
                            }
                        })
                        .ToArray();

                    var socket = await context.WebSockets.AcceptWebSocketAsync();

                    var mres = mref.Invoke(service, mrefParamArray);
                    var cref = mres.GetType().GetMethod("Invoke");
                    var t = (Task)cref.Invoke(mres, new object[] { socket });
                    await t.ConfigureAwait(false);
                }
            });

        }

        /// <summary>
        /// Extract route path from call identifier
        /// </summary>
        private static (Func<string, Func<HttpRequest, HttpResponse, RouteData, Task>, IRouteBuilder>, string)
        ExtractRoutePath(IRouteBuilder router, ICall call)
        {
            switch (call.CallId)
            {
                case PathCallId _:
                    throw new InvalidOperationException("PathCallId path type not set up");

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.DELETE == restCallIdentifier.Method:
                    return (router.MapDelete, restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.GET == restCallIdentifier.Method:
                    return (router.MapGet, restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.PATCH == restCallIdentifier.Method:
                    return ((tmpl, hndlr) => router.MapVerb("PATCH", tmpl, hndlr), restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.POST == restCallIdentifier.Method:
                    return (router.MapPost, restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.PUT == restCallIdentifier.Method:
                    return (router.MapPut, restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.HEAD == restCallIdentifier.Method:
                    return ((tmpl, hndlr) => router.MapVerb("HEAD", tmpl, hndlr), restCallIdentifier.PathPattern);

                // ReSharper disable once PossibleUnintendedReferenceComparison
                case RestCallId restCallIdentifier when Method.OPTIONS == restCallIdentifier.Method:
                    return ((tmpl, hndlr) => router.MapVerb("OPTIONS", tmpl, hndlr), restCallIdentifier.PathPattern);

                case RestCallId _:
                    throw new InvalidOperationException("Unhandled REST Method type for RestCallId");

                case StreamCallId streamCallIdentifier:
                    return (null, streamCallIdentifier.PathPattern);

                default:
                    throw new InvalidOperationException("Unknown type");
            }
        }
    }
}