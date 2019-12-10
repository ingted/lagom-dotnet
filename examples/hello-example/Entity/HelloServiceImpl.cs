// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using wyvern.api.abstractions;
using wyvern.api.hello.Entity.Readside;
using wyvern.api.hello.filters;
using wyvern.api.@internal.readside;
using wyvern.api.@internal.sockets;
using wyvern.api.@internal.surfaces;

namespace wyvern.api.hello
{
    public class HelloServiceImpl : HelloService
    {
        IShardedEntityRegistry Registry { get; }

        ILogger<HelloServiceImpl> Logger { get; }

        ISerializer Serializer { get; }

        ActorSystem ActorSystem { get; }

        public HelloServiceImpl(
            IShardedEntityRegistry registry,
            ILogger<HelloServiceImpl> logger,
            ISerializer serializer,
            ActorSystem actorSystem,
            ReadSide readSide
        )
        {
            Registry = registry;
            Logger = logger;
            ActorSystem = actorSystem;
            Serializer = serializer;
            Registry.Register<HelloEntity>();
            
            // TODO: Ensure this is only registered once
            readSide.Register<HelloEventProcessor, HelloEvent>(
                    () => new HelloEventProcessor()
                );
        }

        public override ServiceCall<NotUsed, string> SayHello(string name)
            => new ServerServiceCall<NotUsed, string>(
                async _ =>
                {
                    var entity = Registry.RefFor<HelloEntity>(name);
                    var response = await entity.Ask(new HelloCommand.SayHelloCommand(name));
                    return response;
                }
            );

        public override ServiceCall<NotUsed, string> SayHelloAuthenticated()
            => Filters.Authenticated(
                async user =>
                    await Task.FromResult(
                        new ServerServiceCall<NotUsed, string>(
                            async _ =>
                            {
                                var entity = Registry.RefFor<HelloEntity>(user.Name);
                                var response = await entity.Ask(new HelloCommand.SayHelloCommand(user.Name));
                                return response;
                            }
                        )
                    )
            );

        public override ServiceCall<UpdateGreetingRequest, string> UpdateGreeting(string name)
            => new ServiceCall<UpdateGreetingRequest, string>(
                async req =>
                {
                    var entity = Registry.RefFor<HelloEntity>(name);
                    var response = await entity.Ask<string>(new HelloCommand.UpdateGreetingCommand(name, req.Message));
                    return response;
                }
            );

        public override ServiceCall<WebSocket, Done> HelloNameStream(string id, long st, long ed)
            => new ServiceCall<WebSocket, Done>(
                async webSocket =>
                {
                    await WebSocketProducer.EntityStreamWithOffset<HelloEvent>(
                            webSocket,
                            Registry.EventStream<HelloEvent>
                        )
                        .Select(
                            id, st, ed,
                            env =>
                            {
                                // TODO: Transformation.
                                var message = new
                                {
                                    env.EntityId,
                                    EventArgs = env.Event
                                };
                                var obj = JsonConvert.SerializeObject(message);
                                var msg = Encoding.ASCII.GetBytes(obj);
                                return msg;
                            },
                            ActorSystem.Materializer()
                        );
                    return Done.Instance;
                }
            );

        public override ServiceCall<WebSocket, Done> HelloStream(long st)
            => new ServiceCall<WebSocket, Done>(
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
                                // TODO: Transformation.
                                var message = new
                                {
                                    EntityId = env.EntityId,
                                    EventArgs = env.Event
                                };
                                var obj = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                                var msg = Encoding.ASCII.GetBytes(obj);
                                return msg;
                            },
                            ActorSystem.Materializer()
                        );
                    return Done.Instance;
                });


        public override Topic<HelloEvent> GreetingsTopic() =>
            TopicProducer.SingleStreamWithOffset<HelloEvent>(
                fromOffset =>
                {
                    var stream = (Registry as IShardedEntityRegistry1)
                        .EventStream<HelloEvent>(
                            HelloEvent.Tag, fromOffset
                        );

                    stream.Select(envelope =>
                    {
                        var (@event, offset) = envelope;
                        var offsetValue = ((Sequence)offset).Value.ToString();
                        var message = new DateTime();
                        return KeyValuePair.Create(message, offset);
                    });

                    return stream;
                }
            );
    }

    public class TopicMessage<T> where T : class
    {
        public string MessageId { get; }
        public T Payload { get; }

        public TopicMessage(string messageId, T payload)
        {
            MessageId = messageId;
            Payload = payload;
        }
    }
}