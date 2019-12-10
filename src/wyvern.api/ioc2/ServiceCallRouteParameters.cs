// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using wyvern.api.abstractions;

namespace wyvern.api.ioc2
{
    public static partial class ReactiveServiceRouteBuilder
    {
        public class ServiceCallRouteParameters
        {
            /// <summary>
            /// Static instantiation factory for ServiceCallRouteParameters a
            /// given ICall implementation
            /// </summary>
            /// <param name="call"></param>
            /// <returns></returns>
            public static ServiceCallRouteParameters Parse(ICall call)
                => new ServiceCallRouteParameters(call);

            /// <summary>
            /// Service route delegate for invocation after pattern mapping
            /// and type binding
            /// </summary>
            /// <value></value>
            public Delegate ServiceRoute { get; }

            /// <summary>
            /// Reference to method information for the targeted service route
            /// </summary>
            /// <value></value>
            public MethodInfo ServiceRouteMethod { get; }

            /// <summary>
            /// Parameters fo the service route which conform to the standard
            /// dotnet route builder pattern matching syntax.
            /// </summary>
            /// <value></value>
            public ParameterInfo[] ServiceRouteMethodParameters { get; }

            /// <summary>
            /// Reference type for the service call signature including the
            /// request and response type as generic arguments.
            /// </summary>
            /// <value></value>
            public Type ServiceCallType { get; }

            /// <summary>
            /// Request type.
            ///
            /// NOTE: If request method is GET, then the type will be `NotUsed`.
            /// Otherwise, it will be the request body object for the given service
            /// call mapping.
            /// </summary>
            /// <value></value>
            public Type RequestType { get; }

            ServiceCallRouteParameters(ICall call)
            {
                ServiceRoute = call.MethodRef;
                ServiceRouteMethod = call.MethodRef.Method;
                ServiceRouteMethodParameters = ServiceRouteMethod.GetParameters();
                ServiceCallType = ServiceRouteMethod.ReturnType;
                RequestType = ServiceCallType.GenericTypeArguments.First();
            }

            /// <summary>
            /// Dynamically create a parameter array for the incoming request to the
            /// target service call.
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            internal object[] CreateParameterArray(RouteData data)
            {
                return ServiceRouteMethodParameters.Select(x =>
                    {
                        var parameterType = x.ParameterType;
                        var name = x.Name;
                        try
                        {
                            var val = data.Values[name].ToString();
                            if (parameterType == typeof(String))
                                return val;
                            if (parameterType == typeof(Int64))
                                return Int64.Parse(val) as object;
                            if (parameterType == typeof(Int32))
                                return Int32.Parse(val) as object;
                            if (parameterType == typeof(Int16))
                                return Int16.Parse(val) as object;

                            throw new Exception("Unsupported path parameter type: " + parameterType.Name);
                        }
                        catch (Exception)
                        {
                            throw new Exception($"Failed to match URL parameter [{name}] in path template.");
                        }
                    })
                    .ToArray();
            }
        }

    }
}
