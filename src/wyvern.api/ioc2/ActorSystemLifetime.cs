using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Microsoft.AspNetCore.Hosting;
using wyvern.api.@internal.readside;
using static wyvern.api.@internal.readside.ClusterDistributionExtensionProvider;

/// <summary>
/// Instantiates an actor system within scope of a configuration laoder
/// </summary>
public class ActorSystemLifetime
{
    IApplicationLifetime AppLifetime { get; }
    ConfigurationLoader ConfigLoader { get; }

    Dictionary<Type, Type> WithExtensions { get; } = new Dictionary<Type, Type>();

    public ActorSystemLifetime(IApplicationLifetime appLifetime, ConfigurationLoader configLoader)
    {
        AppLifetime = appLifetime;
        ConfigLoader = configLoader;
    }

    public ActorSystemLifetime WithExtension<T, TI>()
    {
        WithExtensions.Add(typeof(T), typeof(TI));
        return this;
    }

    public ActorSystem CreateActorSystem()
    {
        StandardOutLogger.InfoColor = ConsoleColor.Cyan;
        StandardOutLogger.DebugColor = ConsoleColor.DarkGray;

        var config = ConfigLoader.Load();
        var name = config.GetString("wyvern.cluster-system-name", "ClusterSystem");
        var actorSystem = ActorSystem.Create(name, config);
        actorSystem.WithExtension<ClusterDistribution, ClusterDistributionExtensionProvider>();
        AppLifetime.ApplicationStopping.Register(() =>
        {
            if (actorSystem != null)
            {
                actorSystem.Log.Warning("Host terminating, starting coordinated shutdown on this node");
                var shutdownTask = CoordinatedShutdown
                        .Get(actorSystem)
                        .Run(CoordinatedShutdown.ClrExitReason.Instance);
                shutdownTask.Wait();
            }
        });
        return actorSystem;
    }
}
