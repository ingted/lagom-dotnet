using System;
using System.Collections.Generic;
using Akka.Actor;
using wyvern.api.@internal.sharding;
using wyvern.entity.command;
using wyvern.entity.@event;
using wyvern.entity.@event.aggregate;
using wyvern.entity.state;
using Microsoft.Extensions.Configuration;
using wyvern.api.abstractions;
using wyvern.api.@internal.readside;
using static wyvern.api.@internal.readside.ClusterDistributionExtensionProvider;
using Akka.Configuration;

namespace wyvern.api.ioc
{
    internal sealed class ShardedEntityRegistryBuilder : IShardedEntityRegistryBuilder
    {
        private ActorSystem ActorSystem { get; }
        private IConfiguration Config { get; }
        private Config Config2 { get; }

        private List<Action<IServiceProvider, IShardedEntityRegistry>> RegistryDelegates { get; } = new List<Action<IServiceProvider, IShardedEntityRegistry>>();
        private List<Action<ReadSide>> ReadSideDelegates { get; } = new List<Action<ReadSide>>();
        private List<Action<ActorSystem>> ExtensionDelegates { get; } = new List<Action<ActorSystem>>();

        public ShardedEntityRegistryBuilder(ActorSystem actorSystem, IConfiguration config, Config config2)
        {
            ActorSystem = actorSystem;
            Config = config;
            Config2 = config2;
            ExtensionDelegates.Add(x => x.WithExtension<ClusterDistribution, ClusterDistributionExtensionProvider>());
        }

        /// <summary>
        /// Register with DI
        /// </summary>
        /// <param name="entityFactory"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public IShardedEntityRegistryBuilder WithShardedEntity<T, C, E, S>(Func<IServiceProvider, T> entityFactory = null)
            where T : ShardedEntity<C, E, S>
            where C : AbstractCommand
            where E : AbstractEvent
            where S : AbstractState
        {
            RegistryDelegates.Add(
                (x, y) =>
                    y.Register<T, C, E, S>(
                        () =>
                            {
                                if (entityFactory == null)
                                    return Activator.CreateInstance<T>();
                                return entityFactory.Invoke(x);
                            }
                    )
            );
            return this;
        }

        /// <summary>
        /// Register without DI
        /// </summary>
        /// <param name="entityFactory"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public IShardedEntityRegistryBuilder WithShardedEntity<T, C, E, S>(Func<T> entityFactory = null)
            where T : ShardedEntity<C, E, S>
            where C : AbstractCommand
            where E : AbstractEvent
            where S : AbstractState
        {
            RegistryDelegates.Add(
                (x, y) =>
                    y.Register<T, C, E, S>(
                        () =>
                            {
                                if (entityFactory == null)
                                    return Activator.CreateInstance<T>();
                                return entityFactory.Invoke();
                            }
                    )
            );
            return this;
        }

        public IShardedEntityRegistryBuilder WithReadSide<TE, TP>()
            where TE : AggregateEvent<TE>
            where TP : ReadSideProcessor<TE>, new()
        {
            ReadSideDelegates.Add(
                x => x.Register(() => new TP
                {
                    Config = Config,
                    Config2 = Config2
                })
            );
            return this;
        }

        public IShardedEntityRegistryBuilder WithExtension<T, TI>()
            where T : class, IExtension
            where TI : IExtensionId
        {
            ExtensionDelegates.Add(
                y => y.WithExtension<T, TI>()
            );
            return this;
        }

        public IShardedEntityRegistry Build(IServiceProvider services)
        {
            foreach (var extensionDelegate in ExtensionDelegates)
                extensionDelegate(ActorSystem);

            var registry = new ShardedEntityRegistry(ActorSystem);
            foreach (var registryDelegate in RegistryDelegates)
                registryDelegate.Invoke(services, registry);

            var readsideConfig = new ReadSideConfig(ActorSystem.Settings.Config);
            var readside = new ReadSideImpl(
                ActorSystem,
                readsideConfig,
                registry
            );

            foreach (var readsideDelegate in ReadSideDelegates)
                readsideDelegate.Invoke(readside);

            return registry;
        }
    }
}
