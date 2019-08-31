using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Persistence.SqlServer;
using Akka.Persistence.SqlServer.Journal;
using Akka.Streams.Dsl;
using Akka.Streams.Util;
using wyvern.api.abstractions;
using wyvern.api.@internal.command;
using wyvern.api.ioc;
using wyvern.entity.command;
using wyvern.entity.@event;
using wyvern.entity.@event.aggregate;
using wyvern.entity.state;
using wyvern.utils;

namespace wyvern.api.@internal.sharding
{
    internal class ShardedEntityRegistry : IShardedEntityRegistry, IShardedEntityRegistry2
    {
        /// <summary>
        /// Registry for entities to be processed using the given actor system
        /// </summary>
        /// <param name="actorSystem"></param>
        public ShardedEntityRegistry(ActorSystem actorSystem)
        {
            ActorSystem = actorSystem;

            var persistenceConfig = ActorSystem.Settings.Config
                .WithFallback(@"wyvern.persistence {}")
                .GetConfig("wyvern.persistence");
            AskTimeout = persistenceConfig.GetTimeSpan("ask-timeout", 5.0d.seconds(), false);
            MaxNumberOfShards = Math.Max(persistenceConfig.GetInt("max-number-of-shards", 1), 1);
            var role = persistenceConfig.GetString("run-entities-on-role", "");
            Role = string.IsNullOrWhiteSpace(role) ? Option<string>.None : new Option<string>(role);
            SnapshotAfter = persistenceConfig.GetInt("snapshot-after", 100);

            Sharding = ClusterSharding.Get(ActorSystem);
            ShardingSettings = ClusterShardingSettings
                .Create(ActorSystem)
                .WithRole(Role.Value);

            QueryPluginId = new Option<string>(SqlReadJournal.Identifier);

            var persistence = SqlServerPersistence.Get(ActorSystem);
            EventsByTagQuery = PersistenceQuery.Get(ActorSystem)
                .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);
            EventsByPersistenceIdQuery = PersistenceQuery.Get(ActorSystem)
                .ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier);

            //var journalConfig = persistence.DefaultJournalConfig;
            //var sqlJournal = new SqlServerJournal(journalConfig);

            ExtractShardId = obj =>
            {
                switch (obj)
                {
                    case IShardId i:
                        return $"{i.ShardId}";
                    case CommandEnvelope commandEnvelope:
                        return commandEnvelope.EntityId.ToShardId(MaxNumberOfShards);
                    default:
                        throw new InvalidOperationException("Cannot derive shard identifier from unknown type");
                }
            };
            ExtractEntityId = obj =>
            {
                switch (obj)
                {
                    case CommandEnvelope commandEnvelope:
                        return ($"{commandEnvelope.EntityId}", commandEnvelope.Payload).ToTuple();
                    default:
                        throw new InvalidOperationException("Cannot derive entity identifier from unknown type");
                }
            };
        }

        /// <summary>
        /// Plugin id for the query model
        /// </summary>
        /// <returns></returns>
        protected virtual Option<string> QueryPluginId { get; } = new Option<string>();

        /// <summary>
        /// Registry name used for prefixing the shard region name
        /// </summary>
        /// <value></value>
        private Option<string> Name { get; } = Option<string>.None;

        /// <summary>
        /// Underlying actor system
        /// </summary>
        /// <value></value>
        private ActorSystem ActorSystem { get; }

        /// <summary>
        /// Sharding regions
        /// </summary>
        /// <value></value>
        private ClusterSharding Sharding { get; }

        /// <summary>
        /// Global sharding settings
        /// </summary>
        /// <value></value>
        private ClusterShardingSettings ShardingSettings { get; }

        /// <summary>
        /// Delegate for extracting entity id from incoming events
        /// </summary>
        /// <value></value>
        private ExtractEntityId ExtractEntityId { get; }

        /// <summary>
        /// Delegate for extracting shard id from incoming events
        /// </summary>
        /// <value></value>
        private ExtractShardId ExtractShardId { get; }

        /// <summary>
        /// Reference for extracting events by tag
        /// </summary>
        /// <value></value>
        private Option<IEventsByTagQuery> EventsByTagQuery { get; }

        /// <summary>
        /// Reference for extracting events by persistence id
        /// </summary>
        /// <value></value>
        private Option<IEventsByPersistenceIdQuery> EventsByPersistenceIdQuery { get; }

        /// <summary>
        /// Registry of event types to entity names
        /// </summary>
        /// <returns></returns>
        private ConcurrentDictionary<Type, string> ReverseRegister { get; } = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Registry of entity names to entity types
        /// </summary>
        /// <returns></returns>
        private ConcurrentDictionary<string, Type> RegisteredTypeNames { get; } =
            new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Timeout on entity reference Ask
        /// </summary>
        /// <value></value>
        private TimeSpan AskTimeout { get; }

        /// <summary>
        /// Max number of available shards, used in hash based distribution
        /// </summary>
        /// <value></value>
        private int MaxNumberOfShards { get; }

        /// <summary>
        /// Role for the current actor system
        /// </summary>
        /// <value></value>
        private Option<string> Role { get; }

        /// <summary>
        /// Number of events to process before commiting a snapshot
        /// </summary>
        /// <value></value>
        private int SnapshotAfter { get; }

        /// <summary>
        /// Reference to actor system termination
        /// </summary>
        public Task WhenTerminated => ActorSystem.WhenTerminated;

        /// <summary>
        /// Get a reference to the entity with the given entityId
        /// </summary>
        /// <param name="entityId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IShardedEntityReference RefFor<T>(string entityId)
            where T : class
        {
            if (!ReverseRegister.TryGetValue(typeof(T), out var entityName))
                throw new InvalidOperationException($"{typeof(T)} not registered.");

            return new ShardedEntityReference(
                entityId,
                Sharding.ShardRegion(PrependRegistryName(entityName)),
                ActorSystem,
                AskTimeout
            );
        }

        /// <summary>
        /// Request to terminate the actor system
        /// </summary>
        /// <returns></returns>
        public Task Terminate()
        {
            return ActorSystem.Terminate();
        }
        public Source<KeyValuePair<E, Offset>, NotUsed> EventStream<E>(AggregateEventTag aggregateTag, Offset fromOffset)
            where E : AggregateEvent<E>
        {
            if (!EventsByTagQuery.HasValue)
                throw new InvalidOperationException("No support for streaming events by tag");

            var queries = EventsByTagQuery.Value;
            var tag = aggregateTag.Tag;

            Offset MapStartingOffset(Offset o) => o;
            var startingOffset = MapStartingOffset(fromOffset);

            return queries.EventsByTag(tag, startingOffset)
                .Select(env => KeyValuePair.Create(
                    env.Event as E,
                    env.Offset
                ));
        }


        Source<EventStreamElement<TE>, NotUsed> IShardedEntityRegistry2.EventStream<TE>(AggregateEventTag aggregateTag, Offset fromOffset)
        {
            if (!EventsByTagQuery.HasValue)
                throw new InvalidOperationException("No support for streaming events by tag");

            var queries = EventsByTagQuery.Value;
            var tag = aggregateTag.Tag;

            Offset MapStartingOffset(Offset o) => o;
            var startingOffset = MapStartingOffset(fromOffset);

            return queries.EventsByTag(tag, startingOffset)
                .Select(envelope => (EventStreamElement<TE>)envelope); ;
        }

        public Source<KeyValuePair<E, Offset>, NotUsed> EventStream<E>(
           AggregateEventTag aggregateTag,
           string persistenceId,
           Offset fromOffset = null,
           Offset toOffset = null
       )
           where E : AggregateEvent<E>
        {
            if (!EventsByPersistenceIdQuery.HasValue)
                throw new InvalidOperationException(
                    "No support for streaming events by persistence id"
                );

            var queries = EventsByPersistenceIdQuery.Value;
            var tag = aggregateTag.Tag;

            return queries.EventsByPersistenceId(
                    persistenceId,
                    fromOffset == null ?
                        0L :
                        ((Sequence)fromOffset).Value,
                    toOffset == null ?
                        Int64.MaxValue :
                        ((Sequence)toOffset).Value
                )
                .Select(env => KeyValuePair.Create(env.Event as E, env.Offset));
        }

        Source<EventStreamElement<TE>, NotUsed> IShardedEntityRegistry2.EventStream<TE>(AggregateEventTag aggregateTag, string persistenceId, Offset fromOffset, Offset toOffset)
        {
            if (!EventsByPersistenceIdQuery.HasValue)
                throw new InvalidOperationException(
                    "No support for streaming events by persistence id"
                );

            var queries = EventsByPersistenceIdQuery.Value;
            var tag = aggregateTag.Tag;

            return queries.EventsByPersistenceId(
                    persistenceId,
                    fromOffset == null ?
                        0L :
                        ((Sequence)fromOffset).Value,
                    toOffset == null ?
                        Int64.MaxValue :
                        ((Sequence)toOffset).Value
            ).Select(envelope => (EventStreamElement<TE>)envelope);
        }

        public void Register<T>(Func<T> entityFactory = null) {

            var entity = typeof(T);
            var types = entity.BaseType.GenericTypeArguments;
            if (entityFactory != null)
            {
                var registerMethod = typeof(ShardedEntityRegistry)
                    .GetMethods()
                    .Where(x => x.Name == "Register")
                    .First(x => x.GetParameters().Length > 0);
                var generic = registerMethod.MakeGenericMethod(
                    entity, types[0], types[1], types[2]
                );
                generic.Invoke(this, new object[] { entityFactory });
            }
            else
            {
                var registerMethod = typeof(ShardedEntityRegistry)
                    .GetMethod("Register", new Type[] { });
                var generic = registerMethod.MakeGenericMethod(
                    entity, types[0], types[1], types[2]
                );
                generic.Invoke(this, null);
            }

        }

        public void Register<T, TC, TE, TS>()
            where T : ShardedEntity<TC, TE, TS>, new()
            where TC : AbstractCommand
            where TE : AbstractEvent
            where TS : AbstractState
        {
            this.Register<T, TC, TE, TS>(() => new T());
        }


        public void Register<T, TC, TE, TS>(Func<T> entityFactory)
            where T : ShardedEntity<TC, TE, TS>
            where TC : AbstractCommand
            where TE : AbstractEvent
            where TS : AbstractState
        {
            var proto = entityFactory.Invoke();
            var entityTypeName = proto.EntityTypeName;
            var entityClassType = proto.GetType();

            var alreadyRegistered = RegisteredTypeNames.GetOrAdd(entityTypeName, entityClassType);
            if (alreadyRegistered != null && alreadyRegistered != entityClassType)
                throw new InvalidOperationException($"The entity type {nameof(T)} is already registered");

            ReverseRegister.TryAdd(entityClassType, entityTypeName);

            var PassivateAfterIdleTimeout = ActorSystem.Settings.Config.GetConfig("wyvern.persistence")
                .GetTimeSpan("passivate-after-idle-timeout", TimeSpan.FromSeconds(100));

            const string snapshotPluginId = "";
            const string journalPluginId = "";

            JoinCluster(ActorSystem);

            if (Role.ForAll(Cluster.Get(ActorSystem).SelfRoles.Contains))
            {
                ActorSystem.Log.Info("Cluster sharding initialized");
                var props = ShardedEntityActorProps.Create<T, TC, TE, TS>(
                    entityTypeName,
                    Option<string>.None,
                    entityFactory,
                    SnapshotAfter,
                    PassivateAfterIdleTimeout,
                    snapshotPluginId,
                    journalPluginId
                );

                Sharding.Start(
                    entityTypeName,
                    props,
                    ShardingSettings,
                    ExtractEntityId,
                    ExtractShardId
                );
            }
            else
            {
                ActorSystem.Log.Warning("Cluster proxy initialized");
                Sharding.StartProxy(
                    entityTypeName,
                    Role.Value,
                    ExtractEntityId,
                    ExtractShardId
                );
            }
        }

        /// <summary>
        /// Prepends the registry name ot the given entity type name
        /// </summary>
        /// <param name="entityTypeName">Entity type name</param>
        /// <returns></returns>
        /// <remarks>
        /// The output is intended to be used as a unique name for
        /// identifying the shard region
        /// </remarks>
        private string PrependRegistryName(string entityTypeName)
        {
            return (Name.HasValue ? Name.Value + "-" : "") + entityTypeName;
        }

        /// <summary>
        /// Provider for self-joining on cluster registration
        /// </summary>
        /// <param name="actorSystem"></param>
        /// <param name="environment"></param>
        private void JoinCluster(ActorSystem actorSystem)
        {
            var config = actorSystem.Settings.Config.GetConfig("wyvern.cluster");

            var joinSelf = config.GetBoolean("join-self");
            var exitWhenDowned = config.GetBoolean("exit-when-downed");
            var isProduction = "Production".Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                StringComparison.InvariantCultureIgnoreCase
            );

            if (isProduction && joinSelf)
                actorSystem.Log.Warning("join-self should not be enabled in prod");

            var cluster = Cluster.Get(actorSystem);

            if (cluster.Settings.SeedNodes.IsEmpty && joinSelf)
                cluster.Join(cluster.SelfAddress);

            CoordinatedShutdown.Get(actorSystem)
                .AddTask(
                    CoordinatedShutdown.PhaseClusterShutdown,
                    "exit-when-downed",
                    () =>
                    {
                        var reason = CoordinatedShutdown.Get(ActorSystem)
                                         .ShutdownReason == CoordinatedShutdown.ClusterDowningReason.Instance;

                        actorSystem.Log.Info("Shutdown coordinated..");

                        if (exitWhenDowned && reason)
                        {
                            actorSystem.Log.Info("Exiting");
                            Environment.Exit(-1);
                        }

                        return Task.FromResult(Done.Instance);
                    }
                );
        }
    }
}
