using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using wyvern.api.abstractions;
using wyvern.api.@internal.readside;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.hello.Entity.Readside

{
    public class HelloReadSideHandler : ReadSideHandler<HelloEvent>
    {
        private HelloDatabase HelloDatabase { get; }

        internal HelloReadSideHandler()
        {
            HelloDatabase = new HelloDatabase();
        }
        
        public override Task<Done> GlobalPrepare()
        {
            return HelloDatabase.CreateTables();
        }

        public override Task<Offset> Prepare(AggregateEventTag tag)
        {
            return HelloDatabase.LoadOffset(tag);
        }

        public override Flow<EventStreamElement<HelloEvent>, Done, NotUsed> Handle()
        {
            return Flow.Create<EventStreamElement<HelloEvent>>()
                .SelectAsync(1, x => HelloDatabase.HandleEvent(x.Event, x.Offset));
        }
    }
}