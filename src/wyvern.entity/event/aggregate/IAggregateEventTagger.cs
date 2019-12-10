// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.entity.@event.aggregate
{
    public interface IAggregateEventTagger
    {
        Type EventType { get; }
    }
}
