using System;
using System.Threading.Tasks;
using wyvern.api;
using wyvern.utils;

namespace wyvern.examples.filters
{
    public class Filters
    {
        public static ServerServiceCall<TReq, TRes> Authenticated<TReq, TRes>(Func<string, Task<ServerServiceCall<TReq, TRes>>> authenticatedServiceCall)
         => ServerServiceCall<TReq, TRes>.ComposeAsync(
                async (RequestHeader requestHeader) =>
                {
                    var userId = "test";
                    // var userId = requestHeader.Principal.OrElseThrow(
                    //      new Exception("Not authenticated")
                    // ).Name;
                    return await authenticatedServiceCall(userId);
                }
            );
    }
}