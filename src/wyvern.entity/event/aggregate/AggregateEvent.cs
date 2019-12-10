// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.entity.@event.aggregate
{
    public abstract class AggregateEvent<E> : AbstractEvent, IAggregateEvent
        where E : AggregateEvent<E>
    {
        public static readonly AggregateEventTag Tag = AggregateEventTag.Of<E>();
        public virtual IAggregateEventTagger AggregateTag => Tag;
    }
}
