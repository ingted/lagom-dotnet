using System.Collections.Generic;

namespace wyvern.api
{
    public class MessageHeader
    {
        public MessageProtocol Protocol { get; }
        public Dictionary<string, string[]> Headers { get; }

        public MessageHeader(MessageProtocol protocol, Dictionary<string, string[]> headers)
        {
            Protocol = protocol;
            Headers = headers;
        }
    }
}