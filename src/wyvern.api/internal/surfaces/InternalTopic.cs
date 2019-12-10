// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    internal class InternalTopic<Message> : Topic<Message>
    {
        public ITopicId TopicId => throw new NotImplementedException();

        // ImmutableArray<AggregateEventTag<Event>> Tags { get; }
        // (AggregateEventTag<Event>, Offset) ReadSideStream { get; } // output...

        // source [ (meesage, offset), _ ]

        public Subscriber<Message> Subscriber()
        {
            throw new NotImplementedException();
        }
    }
}