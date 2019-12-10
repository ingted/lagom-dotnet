// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Microsoft.Net.Http.Headers;

namespace wyvern.api
{
    public class UserAgentHeaderFilter : IHeaderFilter
    {
        /// <summary>
        /// Transforms the headers being passed from the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestHeader TransformClientRequest(RequestHeader request)
        {
            if (request.Principal.HasValue)
            {
                var principal = request.Principal.Value;
                if (principal is ServicePrincipal servicePrincipal)
                {
                    var name = servicePrincipal.ServiceName;
                    return request.WithHeader(HeaderNames.UserAgent, name);
                }
            }
            return request;
        }

        /// <summary>
        /// Transforms the headers being passed to the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RequestHeader TransformServerRequest(RequestHeader request)
        {
            var userAgent = request.GetHeader(HeaderNames.UserAgent);
            if (userAgent.HasValue)
            {
                return request.WithPrincipal(new Principal(userAgent.Value));
            }
            return request;
        }
    }
}