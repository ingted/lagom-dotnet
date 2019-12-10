// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    public interface Topic<M>
    {
        ITopicId TopicId { get; }
        Subscriber<M> Subscriber();
    }
}