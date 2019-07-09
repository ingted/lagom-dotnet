using System.Threading.Tasks;
using Akka.Actor;
using wyvern.api.@internal.sharding;
using Xunit;
using System.Diagnostics;
using wyvern.api.tests.ioc;

namespace wyvern.api.tests.@internal.sharding
{
    public class ShardedEntityReferenceTests : IClassFixture<EntityRegistryFixture>
    {
        private EntityRegistryFixture EntityRegistryFixture { get; }
        private ShardedEntityRegistry Registry { get; }

        public ShardedEntityReferenceTests(EntityRegistryFixture entityRegistryFixture)
        {
            EntityRegistryFixture = entityRegistryFixture;
            Registry = EntityRegistryFixture.Registry;
        }

        [Theory]
        [InlineData("0000")]
        [InlineData("0001")]
        [InlineData("0002")]
        public async Task Test(string id)
        {
            var entityRef = Registry.RefFor<TestEntity>(id);
            var reply = await entityRef.Ask(new TestCommand.Get());
            Assert.IsType<TestState>(reply);
        }

    }
}
