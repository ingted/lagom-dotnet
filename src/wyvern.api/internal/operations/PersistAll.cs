// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Immutable;
using wyvern.api.abstractions;
using wyvern.entity.@event;
using wyvern.utils;

namespace wyvern.api.@internal.operations
{
    /// <summary>
    /// Aggregate event marker for persistence
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Immutable]
    internal sealed class PersistAll<TEvent> : IPersist<TEvent>
        where TEvent : AbstractEvent
    {
        /// <summary>
        /// Events array
        /// </summary>
        /// <value></value>
        internal ImmutableArray<TEvent> Events { get; }

        /// <summary>
        /// After persist effect
        /// </summary>
        /// <value></value>
        internal Action AfterPersist { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="events"></param>
        /// <param name="afterPersist"></param>
        /// <returns></returns>
        public PersistAll(ImmutableArray<TEvent> events, Action afterPersist) =>
            (Events, AfterPersist) = (events, afterPersist);

    }
}
