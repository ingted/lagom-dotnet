using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using Akka.Streams.Util;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.CodeAnalysis;

namespace wyvern.api
{
    public class MessageHeader
    {
        public MessageProtocol Protocol { get; }
        public ImmutableDictionary<string, string[]> Headers { get; }

        public MessageHeader(MessageProtocol protocol, ImmutableDictionary<string, string[]> headers)
        {
            Protocol = protocol;
            Headers = headers;
        }
    }
}