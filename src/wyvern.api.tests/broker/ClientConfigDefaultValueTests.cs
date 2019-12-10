// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Akka.Configuration;
using Xunit;
using static wyvern.api.@internal.broker.Producer;

namespace wyvern.api.tests.broker
{
    public class ClientConfigDefaultValueTests
    {
        ClientConfig Config { get; }

        public ClientConfigDefaultValueTests()
        {
            Config = new ClientConfig(ConfigurationFactory.Empty, "some-section");
        }

        [Fact]
        public void clientConfig_sets_default_values_for_min_backoff()
        {
            Assert.Equal(ClientConfig.Defaults.MIN_BACKOFF, Config.MinBackoff);
        }

        [Fact]
        public void clientConfig_sets_default_values_for_max_backoff()
        {
            Assert.Equal(ClientConfig.Defaults.MAX_BACKOFF, Config.MaxBackoff);
        }

        [Fact]
        public void clientConfig_sets_default_values_for_random_backoff()
        {
            Assert.Equal(ClientConfig.Defaults.RANDOM_FACTOR, Config.RandomBackoffFactor);
        }
    }
}
