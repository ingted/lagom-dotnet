using wyvern.visualize;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using wyvern.api;
using wyvern.api.ioc;
using wyvern.utils;
using static wyvern.api.ioc.ServiceExtensions;
using Akka.Event;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using wyvern.api.tests;
using wyvern.api.tests.ioc;

public class TestApiFactory<T> : WebApplicationFactory<T>
    where T : class
{
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost.CreateDefaultBuilder()
            .UseStartup<T>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(".");
        base.ConfigureWebHost(builder);
    }
}

public class TestStartup
{
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        services.AddShardedEntities(x =>
        {
            x.WithShardedEntity<TestEntity, TestCommand, TestEvent, TestState>();
        });

        services.AddReactiveServices(x =>
            {
                x.AddReactiveService<TestService, TestServiceImpl>();
            },
            ReactiveServicesOption.WithApi |
            ReactiveServicesOption.WithSwagger |
            ReactiveServicesOption.WithTopics |
            ReactiveServicesOption.WithVisualizer
        );

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseReactiveServices();
    }
}