// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Microsoft.Extensions.Hosting;
using wyvern.api.@internal.readside;
using wyvern.monitoring;
using wyvern.monitoring.statsd;
using static wyvern.api.@internal.readside.ClusterDistributionExtensionProvider;

namespace wyvern.api.ioc2
{
    /// <summary>
    /// Instantiates an actor system within scope of a configuration loader
    /// </summary>
    internal class ActorSystemLifetime
    {
        private IApplicationLifetime AppLifetime { get; }
        
        private Config Config { get; }

        public ActorSystemLifetime(Config config, IApplicationLifetime appLifetime)
        {
            AppLifetime = appLifetime;
            Config = config;
        }
        
        public ActorSystem CreateActorSystem()
        {
            StandardOutLogger.InfoColor = ConsoleColor.Cyan;
            StandardOutLogger.DebugColor = ConsoleColor.DarkGray;
            
            var name = Config.GetString("wyvern.cluster-system-name", "ClusterSystem");
            var actorSystem = ActorSystem.Create(name, Config);
            actorSystem.WithExtension<ClusterDistribution, ClusterDistributionExtensionProvider>();
            
            // TODO: Read things like serialization and journals to get plugins
            
            // Add monitoring if configured
            var host = Config.GetString("statsd.host", null);
            if (host != null)
                ActorMonitoringExtension.RegisterMonitor(
                    actorSystem,
                    new ActorStatsDMonitor(
                        host
                    )
                );

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
}
