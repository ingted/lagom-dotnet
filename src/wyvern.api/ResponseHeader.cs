using System.Collections.Generic;

namespace wyvern.api
{
    public class ResponseHeader : MessageHeader
    {
        public static ResponseHeader OK = new ResponseHeader(200, new MessageProtocol(), new Dictionary<string, string[]>());

        public int Code { get; }

        public ResponseHeader(int code, MessageProtocol protocol, Dictionary<string, string[]> headers) : base(protocol, headers)
        {
            Code = code;
        }
    }
}