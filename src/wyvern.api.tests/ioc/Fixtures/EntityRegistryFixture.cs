// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.@internal.sharding;
using wyvern.api.tests.persistence.Fixtures;
using wyvern.api.tests.TestObjects.TestEntity;

namespace wyvern.api.tests.ioc.Fixtures
{
    public class EntityRegistryFixture : ActorSystemFixture
    {
        internal ShardedEntityRegistry Registry { get; }

        public EntityRegistryFixture()
        {
            Registry = new ShardedEntityRegistry(this.ActorSystem);
            Registry.Register<TestEntity, TestCommand, TestEvent, TestState>(() => new TestEntity(this.ActorSystem));
        }
    }
}