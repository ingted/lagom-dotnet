// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.@internal.surfaces;

namespace wyvern.api.abstractions
{
    public interface ITopicCall
    {
        ITopicId TopicId { get; }
        object TopicHolder { get; }
    }

    public interface ITopicCall<out M> : ITopicCall
    {
        ITopicCall<M> WithTopicHolder(TopicHolder topicHolder);
    }
}
