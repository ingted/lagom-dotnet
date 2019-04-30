using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Streams.Util;

namespace wyvern.api
{
    public class RequestHeader : MessageHeader
    {
        public static RequestHeader DEFAULT = new RequestHeader(
            Method.GET, "/",
            new MessageProtocol(),
            new MessageProtocol[] { },
            Option<Principal>.None,
            ImmutableDictionary<string, string[]>.Empty
        );

        public Method Method { get; }
        public string Url { get; }
        public MessageProtocol[] AcceptedResponseProtocols { get; }
        public Option<Principal> Principal { get; }

        public RequestHeader(
            Method method,
            string url,
            MessageProtocol protocol,
            MessageProtocol[] acceptedResponseProtocols,
            Option<Principal> principal,
            ImmutableDictionary<string, string[]> headers
        ) : base(protocol, headers)
        {
            Method = method;
            Url = url;
            AcceptedResponseProtocols = acceptedResponseProtocols;
            Principal = principal;
        }

        public RequestHeader WithHeaders(string key, string[] value)
        {
            return new RequestHeader(
                Method,
                Url,
                Protocol,
                AcceptedResponseProtocols,
                Principal,
                Headers.Add(key, value)
            );
        }

        public RequestHeader WithPrincipal(Principal principal)
        {
            return new RequestHeader(
                Method,
                Url,
                Protocol,
                AcceptedResponseProtocols,
                principal,
                Headers
            );
        }
    }
}