using System;

namespace wyvern.api.ioc
{
    public static partial class ServiceExtensions
    {
        [Flags]
        public enum ReactiveServicesOption
        {
            None = 0,
            /// <summary>
            /// Enables the REST API
            /// </summary>
            WithApi = 1,
            /// <summary>
            /// Enables Swagger generation on the REST API
            /// </summary>
            WithSwagger = 2,
            /// <summary>
            /// Enables the visualizer
            /// </summary>
            WithVisualizer = 4,
            /// <summary>
            /// Enables publishing to an AMQP topic
            /// </summary>
            WithTopics = 8
        }
    }
}