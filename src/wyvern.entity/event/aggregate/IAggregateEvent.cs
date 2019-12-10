// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.entity.@event.aggregate
{
    /// <summary>
    /// Aggregate event
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IAggregateEvent : IEvent
    {
        /// <summary>
        /// Aggregate tag
        /// </summary>
        /// <value></value>
        IAggregateEventTagger AggregateTag { get; }
    }
}
