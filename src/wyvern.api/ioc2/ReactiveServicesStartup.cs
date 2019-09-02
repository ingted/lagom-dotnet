using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class ReactiveServicesStartup
{
    public ReactiveServicesStartup()
    {

    }

    /// <summary>
    /// Add services
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        // TODO: This isn't as nice as I would like it to be, revisit soon...
        services.AddSingleton<ActorSystemLifetime>();
        services.AddSingleton<ActorSystem>(x => {
            var actorSystem = x.GetService<ActorSystemLifetime>().CreateActorSystem();
            return actorSystem;
        });
    }

    /// <summary>
    /// Configure the HTTP pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.ApplicationServices.GetService<ActorSystemLifetime>();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseWebSockets();

        // TODO: register streams
        // TODO: services by reflection
        // TODO: register remote services.
    }
}
