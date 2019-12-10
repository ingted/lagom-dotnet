// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Net;
using System.Threading.Tasks;
using Akka;
using wyvern.api.abstractions;
using wyvern.utils;

namespace wyvern.api
{
    public class ServiceCall<TReq, TRes>
    {
        protected Func<TReq, Task<TRes>> Func { get; }

        // TODO: Principal

        public ServiceCall(Func<TReq, Task<TRes>> func)
        {
            Func = func;
        }

        /// <summary>
        /// Invoke the service call with the given request.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<TRes> Invoke(TReq req)
        {
            return Func(req);
        }

        // TODO: Invoke for Req that is NotUsed.Instance.

        public virtual ServiceCall<TReq, TRes> HandleRequestHeader(Func<RequestHeader, RequestHeader> handler)
        {
            return this;
        }

        public virtual ServiceCall<TReq, TRes> HandleResponseHeader(Func<ResponseHeader, TRes, TRes> handler)
            => new ServiceCall<TReq, TRes>(
                async req =>
                {
                    var res = await Invoke(req);
                    handler.Invoke(ResponseHeader.OK, res);
                    return res;
                }
            );

        public static implicit operator ServiceCall<TReq, TRes>(Func<TReq, Task<TRes>> func)
        {
            return new ServiceCall<TReq, TRes>(func);
        }
    }
}