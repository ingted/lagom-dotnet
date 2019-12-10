// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.entity.@event.aggregate;

namespace wyvern.api.hello
{
    public abstract class HelloEvent : AggregateEvent<HelloEvent>
    {
        /// <summary>
        /// Allocates the entity tag which delineates the domain activity within
        /// the event stream
        /// </summary>
        /// <returns></returns>
        private static readonly AggregateEventTag Tag = AggregateEventTag.Of<HelloEvent>();

        /// <summary>
        /// Aggregate tag reference
        /// </summary>
        public override IAggregateEventTagger AggregateTag => Tag;
        
        public sealed class GreetingUpdatedEvent : HelloEvent
        {
            public string Message { get; }
            public GreetingUpdatedEvent(string message) => Message = message;
        }
    }
}
