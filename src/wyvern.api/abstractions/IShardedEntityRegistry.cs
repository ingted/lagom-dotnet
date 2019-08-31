using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using wyvern.api.ioc;
using wyvern.entity.command;
using wyvern.entity.@event;
using wyvern.entity.@event.aggregate;
using wyvern.entity.state;

namespace wyvern.api.abstractions
{
    public interface IShardedEntityRegistryBase
    {
        IShardedEntityReference RefFor<T>(
            string entityId
        ) where T : class;

        void Register<T>(Func<T> entityFactory = null);

        Task Terminate();
    }

    public interface IShardedEntityRegistry2 : IShardedEntityRegistryBase
    {
        Source<EventStreamElement<TE>, NotUsed> EventStream<TE>(
            AggregateEventTag instance,
            Offset fromOffset
        ) where TE : AggregateEvent<TE>;

        Source<EventStreamElement<TE>, NotUsed> EventStream<TE>(
            AggregateEventTag aggregateTag,
            string persistenceId,
            Offset fromOffset = null,
            Offset toOffset = null
        ) where TE : AggregateEvent<TE>;
    }

    public interface IShardedEntityRegistry1 : IShardedEntityRegistryBase
    {
        Source<KeyValuePair<TE, Offset>, NotUsed> EventStream<TE>(
            AggregateEventTag instance,
            Offset fromOffset
        ) where TE : AggregateEvent<TE>;

        Source<KeyValuePair<TE, Offset>, NotUsed> EventStream<TE>(
            AggregateEventTag aggregateTag,
            string persistenceId,
            Offset fromOffset = null,
            Offset toOffset = null
        ) where TE : AggregateEvent<TE>;
    }

    public interface IShardedEntityRegistry : IShardedEntityRegistry1, IShardedEntityRegistry2
    {

    }
}
