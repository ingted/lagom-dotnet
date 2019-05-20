using wyvern.visualize;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using wyvern.api;
using wyvern.api.ioc;
using wyvern.utils;
using static wyvern.api.ioc.ServiceExtensions;
using Akka.Event;
using System;
using Newtonsoft.Json.Serialization;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        // Serializer used in API calls (optional, can use default)
        services.AddSingleton<IContractResolver>(
            x => new CamelCasePropertyNamesContractResolver()
        );

        services.AddShardedEntities(x =>
        {
            /* Register your entities here */
            x.WithShardedEntity<HelloEntity, HelloCommand, HelloEvent, HelloState>();
        });

        services.AddReactiveServices(x =>
            {
                // Note: these don't really need to be called, they are the default
                //       I've only added them here as an example
                x.WithTopicSerializer<DefaultSerializer>();
                x.WithMessagePropertyExtractor<DefaultExtractor>();

                /* Register all the services here */
                x.AddReactiveService<HelloService, HelloServiceImpl>();

                /* Any additions to the actor system can be done in here */
                x.AddActorSystemDelegate(ActorVisualizeExtension.InstallVisualizer);
            },
            /*
             * Optionally enable any reactive services options here.
             * Note, for any console apps you can simply use `None`
             */
            ReactiveServicesOption.WithApi |
            ReactiveServicesOption.WithSwagger //|
                                               //ReactiveServicesOption.WithTopics |
                                               //ReactiveServicesOption.WithVisualizer
        );

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        //app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
        app.UseReactiveServices();
    }
}