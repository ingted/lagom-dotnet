// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using wyvern.api.@internal.sharding;
using wyvern.api.ioc;

namespace wyvern.api.tests.ioc
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