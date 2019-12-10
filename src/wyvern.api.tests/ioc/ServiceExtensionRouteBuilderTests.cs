// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace wyvern.api.tests.ioc
{
    public class TestStartup : ReactiveServicesStartup
    {

    }

    [TestCaseOrderer("wyvern.api.tests.ioc.AlphabeticalOrderer", "wyvern.api.tests")]
    public class ServiceExtensionRouteBuilderTests : IClassFixture<TestApiFactory<TestStartup>>
    {
        private WebApplicationFactory<TestStartup> AppFactory { get; }

        public ServiceExtensionRouteBuilderTests(TestApiFactory<TestStartup> appFactory)
        {
            AppFactory = appFactory;
        }

        [Fact(Skip = "No tests here yet")]
        public void reactive_services_initializes_without_exception()
        {
            //AppFactory.Server.Host.Services.GetService();
        }
    }
}