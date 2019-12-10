// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


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
            .UseStartup<T>()
            .ConfigureServices(x =>
            {
                x.AddTransient(_ => new Func<TestEntity>(
                    () => new TestEntity()
                ));
            });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(".");
        base.ConfigureWebHost(builder);
    }
}