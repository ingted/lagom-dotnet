// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.utils;

namespace wyvern.api.@internal.surfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Path call identifier
    /// </summary>
    [Immutable]
    internal sealed class PathCallId : CallId
    {
        public string PathPattern { get; }

        internal PathCallId(string pathPattern)
        {
            PathPattern = pathPattern;
        }

        public override string ToString()
        {
            return PathPattern;
        }

        // TODO: Hash code
    }
}
