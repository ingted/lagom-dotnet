using System;
using Akka.Actor;
using Akka.Streams.Util;
using wyvern.api.ioc;
using wyvern.entity.command;
using wyvern.entity.@event;
using wyvern.entity.state;

// TODO: https://github.com/lagom/lagom/blob/93cbacd98b1950b8f96e0ff3b7d0ea10451f9441/service/scaladsl/kafka/server/src/main/scala/com/lightbend/lagom/internal/scaladsl/broker/kafka/ScaladslRegisterTopicProducers.scala
// TODO: https://github.com/lagom/lagom/blob/master/service/scaladsl/kafka/server/src/main/scala/com/lightbend/lagom/scaladsl/broker/kafka/LagomKafkaComponents.scala

namespace wyvern.api.@internal.sharding
{
    internal static class ShardedEntityActorProps
    {
        /// <summary>
        ///     Props initializer
        /// </summary>
        /// <param name="persistenceIdPrefix"></param>
        /// <param name="entityId"></param>
        /// <param name="entityFactory"></param>
        /// <param name="snapshotAfter"></param>
        /// <param name="passivateAfterIdleTimeout"></param>
        /// <param name="snapshotPluginId"></param>
        /// <param name="journalPluginId"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TS"></typeparam>
        /// <returns></returns>
        internal static Props Create<T, TC, TE, TS>(
            string persistenceIdPrefix,
            Option<string> entityId,
            Func<T> entityFactory,
            int snapshotAfter,
            TimeSpan passivateAfterIdleTimeout,
            string snapshotPluginId,
            string journalPluginId)
            where T : ShardedEntity<TC, TE, TS>, new()
            where TC : AbstractCommand
            where TE : AbstractEvent
            where TS : AbstractState
        {
            return Props.Create(() =>
                new ShardedEntityActor<T, TC, TE, TS>(
                    persistenceIdPrefix,
                    entityId,
                    snapshotAfter,
                    passivateAfterIdleTimeout,
                    snapshotPluginId,
                    journalPluginId
                )
            );
        }
    }
}