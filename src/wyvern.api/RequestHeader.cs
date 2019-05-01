using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Streams.Util;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace wyvern.api
{
    public interface IPrincipal
    {
        string Name { get; }
    }

    public class ServicePrincipal : Principal
    {
        public string ServiceName => Name;
        public bool Authenticated { get; }

        public ServicePrincipal(string name) : base(name)
        {
        }
    }

    public class Principal : IPrincipal
    {
        public string Name { get; }

        public Principal(string name)
        {
            Name = name;
        }
    }

    public class RequestHeader : MessageHeader
    {
        public static RequestHeader DEFAULT = new RequestHeader(
            Method.GET, "/",
            new MessageProtocol(),
            new MessageProtocol[] { },
            Option<IPrincipal>.None,
            ImmutableDictionary<string, string[]>.Empty
        );

        public Method Method { get; }
        public string Url { get; }
        public MessageProtocol[] AcceptedResponseProtocols { get; }
        public Option<IPrincipal> Principal { get; }

        public RequestHeader(
            Method method,
            string url,
            MessageProtocol protocol,
            MessageProtocol[] acceptedResponseProtocols,
            Option<IPrincipal> principal,
            ImmutableDictionary<string, string[]> headers
        ) : base(protocol, headers)
        {
            Method = method;
            Url = url;
            AcceptedResponseProtocols = acceptedResponseProtocols;
            Principal = principal;
        }

        public Option<string> GetHeader(string key)
        {
            if (!Headers.TryGetValue(key, out var values))
                return Option<string>.None;
            return values[0];
        }

        public RequestHeader WithHeaders(string key, string[] values)
        {
            return new RequestHeader(
                Method,
                Url,
                Protocol,
                AcceptedResponseProtocols,
                Principal,
                Headers.Add(key, values)
            );
        }

        public RequestHeader WithHeader(string key, string value)
        {
            return new RequestHeader(
                Method,
                Url,
                Protocol,
                AcceptedResponseProtocols,
                Principal,
                Headers.Add(key, new string[] { value })
            );
        }

        public RequestHeader WithPrincipal(IPrincipal principal)
        {
            return new RequestHeader(
                Method,
                Url,
                Protocol,
                AcceptedResponseProtocols,
                new Option<IPrincipal>(principal),
                Headers
            );
        }
    }
}