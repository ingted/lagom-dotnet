// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using wyvern.api.@internal.surfaces;
using System.Text.RegularExpressions;
using System.Text;
using wyvern.api.ioc2;

namespace wyvern.api.ioc
{
    /// <summary>
    /// Main component responsible for generating swagger documents from
    /// the service descriptors.
    /// </summary>
    internal class ReactiveServicesApiDescriptionsDocumentFilter : IDocumentFilter
    {
        IServiceProvider _provider;

        public ReactiveServicesApiDescriptionsDocumentFilter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var schemaRegistry = context.SchemaRegistry;
            var serviceTypes = ReactiveReflector.GetLocalServiceTypes();
            foreach (var serviceType in serviceTypes)
            {
                var instance = _provider.GetService(serviceType);
                var service = (Service)instance;
                foreach (var call in service.Descriptor.Calls)
                {
                    var restCall = call.CallId as RestCallId;
                    if (restCall == null) continue; // SocketCall, PathCall

                    var parameters = Regex.Matches(restCall.PathPattern, "\\{([^\\}]*)\\}")
                            .Select(match => match.Value.Substring(1, match.Value.Length - 2))
                            .Select(x =>
                            {
                                var parts = x.Split(":");
                                var type = parts.Length > 1 ? parts[1] : "string";
                                if (type == "int")
                                {
                                    type = "integer";
                                    // TODO: check min/max values for 64bit
                                }
                                return new NonBodyParameter()
                                {
                                    Name = parts[0],
                                    In = "path",
                                    Required = true,
                                    Type = type,
                                    Format = type == "integer" ? "Int32" : null
                                } as IParameter;
                            })
                            .ToList();

                    var mref = call.MethodRef;
                    var reqType = mref.Method.ReturnType.GenericTypeArguments[0];

                    if (reqType != typeof(NotUsed))
                    {
                        var reqSchema = schemaRegistry.GetOrRegister(reqType);
                        parameters = parameters.Concat(
                                new IParameter[] {
                                    new BodyParameter
                                    {
                                        Schema = reqSchema,
                                        Name = reqType.Name,
                                        Required = true,
                                        Description = reqType.Name
                                    }
                                }
                            ).ToList();
                    }

                    var resType = mref.Method.ReturnType
                        .GenericTypeArguments[1];
                    var method_ref_name = call.MethodRef.Method.Name;
                    var operation = new Operation()
                    {
                        OperationId = method_ref_name,
                        Consumes = new List<string>() {
                            "application/json"
                        },
                        Produces = new List<string>() {
                            "application/json"
                        },
                        Responses = new Dictionary<string, Response>()
                        {
                            {
                                "200", new Response {
                                    Schema = schemaRegistry.GetOrRegister(resType),
                                    Description = $"Returns {resType.Name}"
                                }
                            }
                        },
                        Tags = new[] { service.Descriptor.Name },
                        Parameters = parameters
                    };

                    Func<String, String> removeTypes = (string str) =>
                        {
                            var sb = new StringBuilder();
                            bool capturing = true;
                            bool thinking = false;
                            foreach (var c in str)
                            {
                                if (thinking)
                                {
                                    if (c == '}')
                                    {
                                        thinking = false;
                                        capturing = true;
                                    }
                                    else if (c == ':')
                                    {
                                        capturing = false;
                                        continue;
                                    }
                                }
                                if (c == '{')
                                    thinking = true;
                                if (capturing)
                                    sb.Append(c);
                            }
                            return sb.ToString();
                        };

                    var newPath = removeTypes(restCall.PathPattern);

                    var exists = swaggerDoc.Paths.ContainsKey(newPath);
                    var path = exists ? swaggerDoc.Paths[newPath] : new PathItem();
                    if (!exists)
                    {
                        swaggerDoc.Paths.Add(
                            newPath,
                            path
                        );
                    }

                    /*
                     * Register each method against the existing path dictionary which also
                     * preforms checking for duplicate paths
                     */
                    if (restCall.Method == Method.DELETE)
                    {
                        if (path.Delete != null) throw new InvalidOperationException("Duplicate path");
                        path.Delete = operation;
                    }
                    else if (restCall.Method == Method.GET)
                    {
                        if (path.Get != null) throw new InvalidOperationException("Duplicate path");
                        path.Get = operation;
                    }
                    else if (restCall.Method == Method.POST)
                    {
                        if (path.Post != null) throw new InvalidOperationException("Duplicate path");
                        path.Post = operation;
                    }
                    else if (restCall.Method == Method.PATCH)
                    {
                        if (path.Patch != null) throw new InvalidOperationException("Duplicate path");
                        path.Patch = operation;
                    }
                    else if (restCall.Method == Method.PUT)
                    {
                        if (path.Put != null) throw new InvalidOperationException("Duplicate path");
                        path.Put = operation;
                    }
                }
            }
        }
    }
}