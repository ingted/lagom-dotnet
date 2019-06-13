using Akka.Persistence.Query;
using wyvern.api.@internal.sharding;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.abstractions
{
    public class EventStreamElement<TE>
        where TE : AggregateEvent<TE>
    {
        public string EntityId { get; }
        public TE Event { get; }
        public Offset Offset { get; }

        public EventStreamElement(string entityId, TE @event, Offset offset)
        {
            EntityId = entityId;
            Event = @event;
            Offset = offset;
        }

        public static implicit operator EventStreamElement<TE>(EventEnvelope env)
        {
            return new EventStreamElement<TE>(
                env.PersistenceId.Substring(env.PersistenceId.IndexOf(ShardedEntityActor.Separator) + 1),
                env.Event as TE,
                env.Offset
            );
        }
    }
}
