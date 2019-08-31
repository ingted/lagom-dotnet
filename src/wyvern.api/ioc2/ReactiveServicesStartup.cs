using System.IO;
using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Event;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using wyvern.api;
using wyvern.api.abstractions;
using wyvern.api.@internal.readside;
using wyvern.api.@internal.sharding;
using wyvern.bootstrap.Docker;
using static wyvern.api.@internal.readside.ClusterDistributionExtensionProvider;

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
        services.ConfigureReactiveServicesApi();
        services.AddActorSystem()
                .AddReactiveComponents();
    }

    /// <summary>
    /// Configure the HTTP pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseWebSockets();
        app.UseReactiveServicesRouter();

        // TODO: register streams
        // TODO: services by reflection
        // TODO: register remote services.
    }
}
