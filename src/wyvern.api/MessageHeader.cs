using System.Collections.Immutable;

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