using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using wyvern.api.ioc;

namespace wyvern.api.tests.ioc
{
    public class RouteAnalyzerFixture
    {
        private ServiceCollection ServiceCollection { get; }

        private readonly ServiceExtensions.ReactiveServicesOption _serviceOption;
        private readonly int _count;
        private readonly Action<RouteCollection> _analyzer;

        public RouteAnalyzerFixture(ServiceExtensions.ReactiveServicesOption option, int count, Action<RouteCollection> analyzer = null)
        {
            ServiceCollection = new ServiceCollection();
            _analyzer = analyzer;
            _serviceOption = option;
            _count = count;
        }


        public void Build()
        {
            ServiceCollection.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

            ServiceCollection.AddShardedEntities(x =>
            {
                x.WithShardedEntity<TestEntity, TestCommand, TestEvent, TestState>();
            });

            ServiceCollection.AddReactiveServices(x =>
                {
                    x.AddReactiveService<TestService, TestServiceImpl>();
                },
                _serviceOption
            );

            var routeConsumer = new Mock<ServiceExtensions.IRouteCollectionConsumer>();
            ServiceCollection.AddTransient(services =>
            {
                if (_analyzer != null)
                    routeConsumer
                        .Setup(consumer => consumer.Consume(It.IsAny<RouteCollection>()))
                        .Callback(_analyzer);
                return routeConsumer.Object;
            });

            ServiceCollection.AddLogging();

            var appServices = ServiceCollection.BuildServiceProvider();
            var builder = new Mock<IApplicationBuilder>();
            builder.Setup(x => x.ApplicationServices)
                .Returns(appServices);

            var app = builder.Object;
            app.UseReactiveServices();

            if (_analyzer != null && _count > 0)
                routeConsumer.Verify(consumer => consumer.Consume(It.IsAny<RouteCollection>()), Times.Once());

        }
    }
}