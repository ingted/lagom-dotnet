using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using wyvern.api;
using wyvern.api.abstractions;
using wyvern.api.@internal.surfaces;

public static partial class ReactiveServiceRouteBuilder
{
    public static ServiceCallRouter ExtractRoutePath(ICall call)
    {
        switch (call.CallId)
        {
            case PathCallId _:
                throw new InvalidOperationException("PathCallId path type not set up");

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.DELETE == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => router.MapDelete);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.GET == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => router.MapGet);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.PATCH == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => (tmpl, hndlr) => router.MapVerb("PATCH", tmpl, hndlr));

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.POST == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => router.MapPost);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.PUT == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => router.MapPut);

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.HEAD == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => (tmpl, hndlr) => router.MapVerb("HEAD", tmpl, hndlr));

            // ReSharper disable once PossibleUnintendedReferenceComparison
            case RestCallId restCallIdentifier when Method.OPTIONS == restCallIdentifier.Method:
                return new ServiceCallRouter(restCallIdentifier.PathPattern, router => (tmpl, hndlr) => router.MapVerb("OPTIONS", tmpl, hndlr));

            case RestCallId _:
                throw new InvalidOperationException("Unhandled REST Method type for RestCallId");

            case StreamCallId streamCallIdentifier:
                return new ServiceCallRouter(streamCallIdentifier.PathPattern);

            default:
                throw new InvalidOperationException("Unknown type");
        }
    }

}
