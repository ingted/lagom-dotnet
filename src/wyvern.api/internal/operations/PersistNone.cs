// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.abstractions;
using wyvern.entity.@event;
using wyvern.utils;

namespace wyvern.api.@internal.operations
{
    /// <summary>
    /// Read-only event marker
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    [Immutable]
    internal sealed class PersistNone<TEvent> : IPersist<TEvent>
        where TEvent : AbstractEvent
    {
    }
}
