using System;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wyvern.api.abstractions;
using wyvern.api.ioc;

namespace wyvern.api
{
    public static class ShardedEntitiesModule
    {
        public static IServiceCollection AddShardedEntities(this IServiceCollection serviceCollection,
            Action<IShardedEntityRegistryBuilder> builderDelegate)
        {
            serviceCollection.AddSingleton(services =>
            {
                // TODO: this can just be registered and invoked on the other end...
                var actorSystem = services.GetService<ActorSystem>();
                // TODO: Clean up config so it's directed.
                var builder = new ShardedEntityRegistryBuilder(actorSystem, services.GetService<IConfiguration>(), services.GetService<Config>());

                builderDelegate.Invoke(builder);

                return builder.Build(services);
            });

            return serviceCollection;
        }
    }
}
