// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    /// <summary>
    /// Call identifier base
    /// </summary>
    /// <remarks>
    /// We keep this untyped to obscure the underlying implementation
    /// </remarks>
    internal abstract class CallId : ICallId
    {
    }
}
