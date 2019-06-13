using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using wyvern.api.abstractions;
using wyvern.entity.@event;
using wyvern.entity.@event.aggregate;

public static class WebSocketProducer
{
    public static EntityWebSocketProducer<TE> EntityStreamWithOffset<TE>(
        WebSocket websocket,
        Func<AggregateEventTag, string, Offset, Offset, Source<EventStreamElement<TE>, NotUsed>> streamSource
    )
        where TE : AggregateEvent<TE>
    {
        return new EntityWebSocketProducer<TE>(
            websocket,
            streamSource
        );
    }

    public static WebSocketProducer<TE> StreamWithOffset<TE>(
        WebSocket websocket,
        Func<AggregateEventTag, Offset, Source<EventStreamElement<TE>, NotUsed>> streamSource
    )
        where TE : AggregateEvent<TE>
    {
        return new WebSocketProducer<TE>(
            websocket,
            streamSource
        );
    }
}

public class WebSocketProducer<TE>
    where TE : AggregateEvent<TE>
{
    WebSocket WebSocket { get; }
    Func<AggregateEventTag, Offset, Source<EventStreamElement<TE>, NotUsed>> StreamSource { get; }

    public WebSocketProducer(
        WebSocket websocket,
        Func<AggregateEventTag, Offset, Source<EventStreamElement<TE>, NotUsed>> streamSource
    )
    {
        WebSocket = websocket;
        StreamSource = streamSource;
    }

    public async Task Select(
        long offset,
        Func<EventStreamElement<TE>, byte[]> func,
        ActorMaterializer materializer)
    {
        var buffer = new byte[1024 * 4];
        var result = await WebSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        await StreamSource.Invoke(
            AggregateEventTag.Of<TE>(),
            Offset.Sequence(offset)
        )
        .RunForeach((envelope) =>
        {
            var m = func(envelope);
            Task.Run(async () =>
            {
                await WebSocket.SendAsync(
                        new ArraySegment<byte>(
                            m,
                            0,
                            m.Length
                        ),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
            });
        }, materializer);

        while (!result.CloseStatus.HasValue)
        {
            result = await WebSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
        }
        await WebSocket.CloseAsync(
            result.CloseStatus.Value,
            result.CloseStatusDescription,
            CancellationToken.None
        );
    }
}
