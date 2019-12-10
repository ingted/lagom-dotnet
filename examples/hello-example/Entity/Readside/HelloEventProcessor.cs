using wyvern.api.@internal.readside;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.hello.Entity.Readside
{
    public class HelloEventProcessor : ReadSideProcessor<HelloEvent>
    {
        public override AggregateEventTag[] AggregateTags => new []
        {
            HelloEvent.Tag // TODO: all tags
        };

        public override ReadSideHandler<HelloEvent> BuildHandler()
        {
            return new HelloReadSideHandler();
        }
    }
}