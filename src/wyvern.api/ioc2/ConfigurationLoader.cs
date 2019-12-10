// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Akka.Bootstrap.Docker;
using Akka.Configuration;
using wyvern.bootstrap.Docker;

public class ConfigurationLoader
{
    internal Config Load()
    {
        // Prepare config root
        var configRoot = ConfigurationFactory.FromResource(
            "wyvern.api.reference.conf",
            Assembly.GetAssembly(typeof(ConfigurationLoader))
        );

        // Load Akka config values from environment variables first
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = entry.Key as string ?? String.Empty;
            if (!key.ToUpper().StartsWith("AKKA:")) continue;
            configRoot.WithFallback(ConfigurationFactory.ParseString(
                $"{key.Replace(":", ".")} = {entry.Value as string ?? String.Empty}"
            ));
        }

        // Load Fallback sources (environment first, then base config)
        var assemblyFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        var environment = Environment.GetEnvironmentVariable("AKKA_ENVIRONMENT");
        var config = (new[]
            {
                (1, "akka.conf"), // Last fallback
                (2, "akka.overrides.conf"), // First fallback
                (3, $"akka.{environment}.conf") // First preference
            })
            .Select(x => (x.Item1, Path.Combine(assemblyFolder, x.Item2)))
            .Where(x => File.Exists(x.Item2))
            .OrderByDescending(x => x.Item1)
            .Aggregate(
                configRoot,
                (acc, cur) => acc.WithFallback(
                    File.ReadAllText(
                        cur.Item2
                    )
                )
            )
            .BootstrapFromDocker(false)
            .BootstrapRolesFromDocker();

        return config;
    }
}
