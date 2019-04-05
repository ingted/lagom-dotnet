using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using Microsoft.Extensions.Logging;
using wyvern.api.abstractions;
using wyvern.api.@internal.surfaces;
using static HelloCommand;
using static HelloEvent;

public class HelloServiceImpl : HelloService
{
    IShardedEntityRegistry Registry { get; }

    ILogger<HelloServiceImpl> Logger { get; }

    ActorSystem ActorSystem { get; }

    public HelloServiceImpl(
            IShardedEntityRegistry registry,
            ILogger<HelloServiceImpl> logger,
            ActorSystem actorSystem
        )
    {
        Registry = registry;
        Logger = logger;
        ActorSystem = actorSystem;
    }

    public override Func<string, Func<NotUsed, Task<string>>> SayHello =>
            name =>
            async _ =>
            {
                var entity = Registry.RefFor<HelloEntity>(name);
                var response = await entity.Ask<string>(new SayHelloCommand(name));
                return response as string;
            };

    public override Func<string, Func<UpdateGreetingRequest, Task<string>>> UpdateGreeting =>
        name =>
        async req =>
        {
            var entity = Registry.RefFor<HelloEntity>(name);
            return await entity.Ask<string>(new UpdateGreetingCommand(name, req.Message));
        };

    public override Func<string, long, long, Func<WebSocket, Task>> HelloNameStream =>
        (id, st, ed) =>
        async webSocket =>
        {
            await WebSocketProducer.EntityStreamWithOffset<HelloEvent>(
                    webSocket,
                    Registry.EventStream<HelloEvent>
                )
                .Select(
                    id, st, ed,
                    (env) =>
                    {
                        var (@event, offset) = env;
                        var message = @event;
                        var obj = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                        var msg = Encoding.ASCII.GetBytes(obj);
                        return msg;
                    },
                    ActorSystem.Materializer()
                );
        };

    public override Func<long, Func<WebSocket, Task>> HelloStream =>
        (st) =>
        async webSocket =>
        {
            await WebSocketProducer.StreamWithOffset<HelloEvent>(
                    webSocket,
                    Registry.EventStream<HelloEvent>
                )
                .Select(
                    st,
                    (env) =>
                    {
                        var (@event, offset) = env;
                        var message = @event;
                        var obj = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                        var msg = Encoding.ASCII.GetBytes(obj);
                        return msg;
                    },
                    ActorSystem.Materializer()
                );
        };

    public override Topic<HelloEvent> GreetingsTopic() =>
        TopicProducer.SingleStreamWithOffset<HelloEvent>(
            fromOffset => Registry.EventStream<HelloEvent>(
                HelloEventTag.Instance, fromOffset
            )
            .Select(envelope =>
            {
                var (@event, offset) = envelope;
                var message = @event;
                return KeyValuePair.Create(message, offset);
            })
        );
}
