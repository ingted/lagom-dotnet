// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using Akka.Configuration;
using Akka.Util;
using wyvern.utils.extensions;

namespace wyvern.api.@internal.readside
{
    internal class ReadSideConfig
    {
        public TimeSpan GlobalPrepareTimeout { get; } = 20d.seconds();
        public TimeSpan MinBackoff { get; } = 3d.seconds();
        public TimeSpan MaxBackoff { get; } = 30d.seconds();
        public TimeSpan OffsetTimeout { get; } = 5d.seconds();
        public double RandomBackoffFactor { get; } = 0.2d;
        public Option<string> Role { get; } = Option<string>.None;

        public ReadSideConfig(Config config)
        {
            var configuration = config
                .WithFallback(
                    ConfigurationFactory.ParseString("wyvern.persistence.read-side {}")
                ).GetConfig("wyvern.persistence.read-side");
            // TODO: Load proper fallback...
            GlobalPrepareTimeout = configuration.GetTimeSpan("global-prepare-timeout", GlobalPrepareTimeout);
            MinBackoff = configuration.GetTimeSpan("failure-exponential-backoff.min", MinBackoff);
            MaxBackoff = configuration.GetTimeSpan("failure-exponential-backoff.max", MaxBackoff);
            OffsetTimeout = configuration.GetTimeSpan("offset-timeout", OffsetTimeout);
            RandomBackoffFactor = configuration.GetDouble("failure-exponential-backoff.random-factor", RandomBackoffFactor);
            var role = configuration.GetString("run-on-role");
            Role = string.IsNullOrWhiteSpace(role) ? Role : new Option<string>(role);
        }
    }
}
