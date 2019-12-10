// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wyvern.api;
using wyvern.utils;

namespace wyvern.examples.filters
{
    public class Filters
    {
        /// <summary>
        /// Sample route filter to compose against a service call
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="authenticatedServiceCall"></param>
        /// <returns></returns>
        public static ServerServiceCall<TReq, TRes> Authenticated<TReq, TRes>(
                Func<User, Task<ServerServiceCall<TReq, TRes>>> authenticatedServiceCall
            ) => ServerServiceCall<TReq, TRes>.ComposeAsync(
                async requestHeader =>
                {
                    var userName = requestHeader.Principal.OrElseThrow(
                        new Exception("Not authenticated")
                    ).Name;
                    var user = new User(userName);
                    return await authenticatedServiceCall(user);
                }
            );
    }
}