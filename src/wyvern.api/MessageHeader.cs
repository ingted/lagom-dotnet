// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using wyvern.utils;

namespace wyvern.api
{
    [Immutable]
    [ExcludeFromCodeCoverage]
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