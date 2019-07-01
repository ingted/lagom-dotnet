using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
