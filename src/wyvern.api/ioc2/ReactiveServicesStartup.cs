using System;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Event;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using wyvern.api;
using wyvern.api.@internal.readside;
using wyvern.api.@internal.sharding;
using wyvern.api.ioc;
using wyvern.bootstrap.Docker;
using static wyvern.api.@internal.readside.ClusterDistributionExtensionProvider;

public class ReactiveReflector
{
    public static Type[] GetEntityTypes()
    {
        var entityType = typeof(ShardedEntity);
        var entities = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => entityType.IsAssignableFrom(x))
            .Where(x => x != entityType)
            .Where(x => !x.IsAbstract);
        return entities.ToArray();
    }

    public static Type[] GetRemoteServiceTypes()
    {
        var localBaseTypes = GetLocalServiceTypes()
            .Select(x => x.BaseType);
        var serviceType = typeof(Service);
        var services = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => serviceType.IsAssignableFrom(x))
            .Where(x => x != serviceType)
            .Where(x => x.IsAbstract);
        return services.Where(x => !localBaseTypes.Contains(x)).ToArray();

    }

    public static Type[] GetLocalServiceTypes()
    {
        var serviceType = typeof(Service);
        var services = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => serviceType.IsAssignableFrom(x))
            .Where(x => x != serviceType)
            .Where(x => !x.IsAbstract);
        return services.ToArray();
    }
}

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

        // TODO: Add these somewhere
        //x.WithTopicSerializer<DefaultSerializer>();
        //x.WithMessagePropertyExtractor<DefaultExtractor>();

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
        app.ActivateServiceRegistry();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseWebSockets();
        app.UseReactiveServicesRouter();

        // TODO: register streams
        // TODO: services by reflection
        // TODO: register remote services.
    }
}
