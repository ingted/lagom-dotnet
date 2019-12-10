// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using wyvern.api.abstractions;
using wyvern.entity.@event;
using wyvern.utils;

namespace wyvern.api.@internal.operations
{
    /// <summary>
    /// Event marker for persistence
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Immutable]
    internal sealed class PersistOne<TEvent> : IPersist<TEvent>
        where TEvent : AbstractEvent
    {
        /// <summary>
        /// Event
        /// </summary>
        /// <value></value>
        public TEvent Event { get; }

        /// <summary>
        /// Delegate event wrapper for post-action side effects
        /// </summary>
        /// <value></value>
        public Action<TEvent> AfterPersist { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PersistOne(TEvent @event, Action<TEvent> afterPersist) =>
            (Event, AfterPersist) = (@event, afterPersist);

    }
}
