using System;
using System.Threading.Tasks;

namespace wyvern.api
{
    public class ServerServiceCall<TReq, TRes> : ServiceCall<TReq, TRes>
    {
        Func<RequestHeader, TReq, Task<(ResponseHeader, TRes)>> InvokeHeadersOverride { get; }

        public ServerServiceCall(Func<TReq, Task<TRes>> func) : base(func)
        {
        }

        public ServerServiceCall(Func<RequestHeader, TReq, Task<(ResponseHeader, TRes)>> func) : base(null)
        {
            InvokeHeadersOverride = func;
        }

        public static ServerServiceCall<TReq, TRes> Compose(Func<RequestHeader, ServerServiceCall<TReq, TRes>> block)
        {
            return new ServerServiceCall<TReq, TRes>((RequestHeader requestHeader, TReq request) =>
            {
                return block(requestHeader).InvokeWithHeaders(requestHeader, request);
            });
        }

        public static ServerServiceCall<TReq, TRes> ComposeAsync(Func<RequestHeader, Task<ServerServiceCall<TReq, TRes>>> block)
        {
            return new ServerServiceCall<TReq, TRes>(async (RequestHeader requestHeader, TReq request) =>
            {
                var x = await block(requestHeader);
                return await x.InvokeWithHeaders(requestHeader, request);
            });
        }

        public ServerServiceCall(Func<TReq, Task<TRes>> func, Func<RequestHeader, TReq, Task<(ResponseHeader, TRes)>> invokeHeadersOverride) : base(func)
        {
            InvokeHeadersOverride = invokeHeadersOverride;
        }

        public virtual async Task<(ResponseHeader, TRes)> InvokeWithHeaders(RequestHeader header, TReq request)
        {
            if (InvokeHeadersOverride != null)
            {
                var response = await InvokeHeadersOverride(header, request);
                return (ResponseHeader.OK, response.Item2);
            }
            else
            {
                var response = await Invoke(request);
                return (ResponseHeader.OK, response);
            }
        }

        public override ServiceCall<TReq, TRes> HandleRequestHeader(Func<RequestHeader, RequestHeader> handler)
        {
            ServerServiceCall<TReq, TRes> self = this;
            return new ServerServiceCall<TReq, TRes>(
                async (TReq request) =>
                {
                    var requestHeader = handler.Invoke(RequestHeader.DEFAULT);
                    var result = await self.InvokeWithHeaders(requestHeader, request);
                    return result.Item2;
                },
                (RequestHeader requestHeader, TReq request) =>
                {
                    return self.InvokeWithHeaders(handler(requestHeader), request);
                }
            );
        }
    }
}