// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    internal sealed class TopicCall<M> : ITopicCall<M>
    {
        /// <summary>
        /// Topic identifier
        /// </summary>
        /// <value></value>
        public ITopicId TopicId { get; }
        public object TopicHolder { get; }

        internal TopicCall(ITopicId topicId, object topicHolder)
        {
            (TopicId, TopicHolder) =
            (topicId, topicHolder);
        }

        public ITopicCall<M> WithTopicHolder(TopicHolder topicHolder)
        {
            return new TopicCall<M>(TopicId, topicHolder);
        }
    }
}