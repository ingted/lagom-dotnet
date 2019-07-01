using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using wyvern.api.abstractions;

public static partial class ReactiveServiceRouteBuilder
{
    public class ServiceCallRouteParameters
    {
        public static ServiceCallRouteParameters Parse(ICall call) => new ServiceCallRouteParameters(call);

        public Delegate ServiceRoute { get; }
        public MethodInfo ServiceRouteMethod { get; }
        public ParameterInfo[] ServiceRouteMethodParameters { get; }
        public Type ServiceCallType { get; }
        public Type RequestType { get; }

        ServiceCallRouteParameters(ICall call)
        {
            ServiceRoute = call.MethodRef;
            ServiceRouteMethod = call.MethodRef.Method;
            ServiceRouteMethodParameters = ServiceRouteMethod.GetParameters();
            ServiceCallType = ServiceRouteMethod.ReturnType;
            RequestType = ServiceCallType.GenericTypeArguments[0];
        }

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
