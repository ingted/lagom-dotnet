using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Persistence.Query;
using wyvern.api.@internal.surfaces;
using wyvern.api.ioc;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace wyvern.api.tests.ioc
{
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            var result = testCases.ToList();
            result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            return result;
        }
    }

    [TestCaseOrderer("wyvern.api.tests.ioc.AlphabeticalOrderer", "wyvern.api.tests")]
    public class ServiceExtensionRouteBuilderTests
    {
        [Fact]
        public void reactive_services_initializes_without_exception()
        {
            var builder = new RouteAnalyzerFixture(
                ServiceExtensions.ReactiveServicesOption.WithApi | ServiceExtensions.ReactiveServicesOption.WithSwagger,
                0
            );
            builder.Build();
        }

        [Theory]
        [InlineData(ServiceExtensions.ReactiveServicesOption.WithApi | ServiceExtensions.ReactiveServicesOption.WithSwagger, 10)]
        [InlineData(ServiceExtensions.ReactiveServicesOption.None, 0)]
        public void service_calls_are_registered_as_routes(ServiceExtensions.ReactiveServicesOption option, int count)
        {
            var builder = new RouteAnalyzerFixture(
                option,
                count,
                routes =>
                {
                    var routePaths = new string[routes.Count];
                    for (int i = 0; i < routes.Count; i++)
                    {
                        var route = routes[i];
                        routePaths[i] = route.ToString();
                    }

                    Assert.Equal(count, routes.Count);
                    if (option == ServiceExtensions.ReactiveServicesOption.None) return;

                    var paths = new TestServiceImpl().Descriptor.Calls.Select(x => x.CallId)
                        .Cast<RestCallId>()
                        .ToArray()
                        .Select(x => x.PathPattern)
                        .ToArray();

                    Assert.Equal(count, paths.Length);

                    foreach (var path in routePaths)
                    {
                        Assert.Contains(paths, x => x == path);
                    }
                });

            builder.Build();

        }

    }
}