using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Akka;
using wyvern.api;
using wyvern.api.abstractions;
using wyvern.api.@internal.surfaces;

public abstract class HelloService : Service
{
    public class UpdateGreetingRequest
    {
        public string Message { get; set; }
    }

    public abstract ServiceCall<NotUsed, string> SayHello(string name);

    public abstract ServiceCall<NotUsed, string> SayHelloAuthenticated();

    public abstract ServiceCall<UpdateGreetingRequest, string> UpdateGreeting(string name);

    public abstract ServiceCall<WebSocket, Done> HelloNameStream(string id, long st, long ed);

    public abstract ServiceCall<WebSocket, Done> HelloStream(long st);

    public abstract Topic<HelloEvent> GreetingsTopic();

    public override IDescriptor Descriptor =>
        Named("HelloService")
            .WithCalls(
                RestCall(
                    Method.GET,
                    "/api/hello/{name}",
                    (string name) =>
                        SayHello(name)
                ),
                RestCall(
                    Method.GET,
                    "/api/hello",
                    SayHelloAuthenticated
                ),
                RestCall(
                    Method.POST,
                    "/api/hello/{name}",
                    (string name) =>
                        UpdateGreeting(name)
                ),
                StreamCall(
                    "/ws/hello/name",
                    (string id, long st, long ed) =>
                        HelloNameStream(id, st, ed)
                ),
                StreamCall(
                    "/ws/hello",
                    (long st) =>
                        HelloStream(st)
                )
            )
            .WithTopics(
                Topic("greetings-service", GreetingsTopic)
            );
}
