using System.Threading.Tasks;
using Akka.Actor;
using wyvern.api.@internal.sharding;
using Xunit;

namespace wyvern.api.tests.@internal.sharding
{
    public class ShardedEntityReferenceTests : IClassFixture<ActorSystemFixture>
    {
        private ActorSystemFixture ActorSystemFixture { get; }

        public ShardedEntityReferenceTests(ActorSystemFixture actorSystemFixture)
        {
            ActorSystemFixture = actorSystemFixture;
        }

        [Theory]
        [InlineData("0001")]
        public async Task Test(string id)
        {
            var registry = new ShardedEntityRegistry(ActorSystemFixture.ActorSystem);
            registry.Register<TestEntity, TestCommand, TestEvent, TestState>(() => new TestEntity(ActorSystemFixture.ActorSystem));
            var entityRef = registry.RefFor<TestEntity>(id);

            var reply = await entityRef.Ask(new TestCommand.Get());

            Assert.IsType<TestState>(reply);
        }
    }
}
