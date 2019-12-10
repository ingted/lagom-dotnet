// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Akka.Configuration;

namespace wyvern.api.@internal.broker
{
    internal static partial class Producer
    {
        /// <summary>
        /// Producer configuration
        /// </summary>
        public class ProducerConfig : ClientConfig
        {
            /// <summary>
            /// Section path
            /// </summary>
            const string section = "wyvern.broker.servicebus.client.producer";

            /// <summary>
            /// Run on role
            /// </summary>
            /// <value></value>
            public string Role { get; }

            /// <summary>
            ///
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public ProducerConfig(Config config) : base(config, section)
            {
                Role = config.GetString("role", string.Empty);
            }
        }
    }
}
