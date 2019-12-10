using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.hello.Entity.Readside
{
    // TODO: register all readside handlers
    public class HelloDatabase
    {
        public Task<Done> CreateTables()
        {
            return Task.FromResult(Done.Instance);
        }
        
        public Task<Offset> LoadOffset(AggregateEventTag tag) // TODO: Typed agg
        {
            return Task.FromResult(Offset.NoOffset());
        }
        
        public Task<Done> HandleEvent(HelloEvent e, Offset offset)
        {
            return Task.FromResult(Done.Instance);
        }
    }
}