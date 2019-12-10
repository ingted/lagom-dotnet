// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using wyvern.api.tests.TestObjects;
using Xunit;

namespace wyvern.api.tests.ioc
{
    public class UserWithId
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
    }

    public class UserFriend
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Tests : IClassFixture<TestApiFactory<TestStartup>>
    {
        private WebApplicationFactory<TestStartup> AppFactory { get; }

        public Tests(TestApiFactory<TestStartup> appFactory)
        {
            AppFactory = appFactory;
        }

        [Theory]
        [InlineData("/swagger/v1/swagger.json")]
        public async Task api_provides_swagger_generation(string url)
        {
            var client = AppFactory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(str);
        }

        [Fact(Skip = "TODO: iterate through registered routes to ensure all types are registered")]
        public async Task api_routes_registered()
        {
            await Task.CompletedTask;
        }

        [Theory]
        [InlineData("/api/user/test")]
        [InlineData("/api/user/test/friend/test2")]
        public async Task api_accepts_get_requests(string url)
        {
            var client = AppFactory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(str);
        }

        [Theory]
        [InlineData("/api/users")]
        [InlineData("/api/user/test/friends")]
        public async Task api_accepts_post_requests(string url)
        {
            var client = AppFactory.CreateClient();
            var content = new StringContent("{\"name\": \"test_name\"}");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
        }

        [Theory]
        [InlineData("/api/user/test/friend-seq/1")]
        [InlineData("/api/user/test/friend-seq/400")]
        public async Task supports_integer_route_constraints(string url)
        {
            var client = AppFactory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
        }

        [Theory]
        [InlineData("/api/user/test/friend-seq/1-f00")]
        [InlineData("/api/user/test/friend-seq/baz")]
        public async Task integer_route_constraints_must_not_be_strings(string url)
        {
            var client = AppFactory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.Equal(404, (int)response.StatusCode);
        }


        [Theory]
        [InlineData("/api/not-a-real-route")]
        [InlineData("/api/something-they-would-not-use")]
        public async Task unknown_routes_return_404(string url)
        {
            var client = AppFactory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.Equal(404, (int)response.StatusCode);
        }


    }

}
