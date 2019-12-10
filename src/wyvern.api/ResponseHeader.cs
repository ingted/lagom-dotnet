// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Generic;
using System.Collections.Immutable;

namespace wyvern.api
{
    public class ResponseHeader : MessageHeader
    {
        public static ResponseHeader OK = new ResponseHeader(200, new MessageProtocol(), ImmutableDictionary<string, string[]>.Empty);

        public int Code { get; }

        public ResponseHeader(int code, MessageProtocol protocol, ImmutableDictionary<string, string[]> headers) : base(protocol, headers)
        {
            Code = code;
        }
    }
}