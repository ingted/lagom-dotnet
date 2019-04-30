using System.Collections.Generic;
using Akka.Streams.Util;

namespace wyvern.api
{
    public class RequestHeader : MessageHeader
    {
        public static RequestHeader DEFAULT = new RequestHeader(
            Method.GET, "/",
            new MessageProtocol(),
            new MessageProtocol[] { },
            //Option<Principal>.None,
            new Option<Principal>(new api.Principal("jeff")), // HACK:
            new Dictionary<string, string[]>()
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
            Dictionary<string, string[]> headers
        ) : base(protocol, headers)
        {
            Method = method;
            Url = url;
            AcceptedResponseProtocols = acceptedResponseProtocols;
            Principal = principal;
        }
    }
}