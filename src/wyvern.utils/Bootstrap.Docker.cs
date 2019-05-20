using System;
using System.Linq;
using System.Net;
using Akka.Configuration;

namespace wyvern.bootstrap.Docker
{
    /// <summary>
    ///     Modifies our HOCON configuration based on environment variables
    ///     supplied by Docker.
    /// </summary>
    public static class WyvernDockerBootstrap
    {
        /// <summary>
        ///     Extension method intended to chain configuration derived from
        ///     Docker-supplied environment variables to the front of the fallback chain,
        ///     overriding any values that were provided in a built-in HOCON file.
        /// </summary>
        /// <param name="input">The current configuration object.</param>
        /// <param name="assignDefaultHostName">If set to <c>true</c>, bootstrapper will use <see cref="Dns.GetHostName"/> to assign a default hostname
        /// in the event that the CLUSTER_IP environment variable is not specified. If <c>false</c>, then we'll leave it blank.</param>
        /// <returns>An updated Config object with <see cref="input" /> chained behind it as a fallback. Immutable.</returns>
        /// <example>
        ///     var config = HoconLoader.FromFile("myHocon.hocon");
        ///     var myActorSystem = ActorSystem.Create("mySys", config.BootstrapFromDocker());
        /// </example>
        public static Config BootstrapRolesFromDocker(this Config input)
        {
            var clusterRoles = Environment.GetEnvironmentVariable("CLUSTER_ROLES")?.Trim();

            if (string.IsNullOrEmpty(clusterRoles))
            {
                Console.WriteLine($"[wyvern-Bootstrap] Environment variable CLUSTER_ROLES was not set.");
                return input;
            }

            // Don't have access to Akka.NET ILoggingAdapter yet, since ActorSystem isn't started.
            Console.WriteLine($"[wyvern-Bootstrap] ROLES={clusterRoles}");


            if (!string.IsNullOrEmpty(clusterRoles))
            {
                var seeds = clusterRoles.Split(",");
                var injectedRoleConfigString =
                    "akka.cluster.roles=[\"" +
                        String.Join("\",\"", seeds) +
                    "\"]";
                input = ConfigurationFactory.ParseString(
                    injectedRoleConfigString
                )
                .WithFallback(input);

            }

            return input;
        }
    }
}