// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace wyvern.api.ioc2
{
    public class ServiceCallRouter
    {
        public string PathPattern { get; }
        public Func<IRouteBuilder, Func<string, Func<HttpRequest, HttpResponse, RouteData, Task>, IRouteBuilder>> Map { get; }

        public ServiceCallRouter(string pathPattern, Func<IRouteBuilder, Func<string, Func<HttpRequest, HttpResponse, RouteData, Task>, IRouteBuilder>> map = null)
        {
            PathPattern = pathPattern;
            Map = map;
        }
    }
}
