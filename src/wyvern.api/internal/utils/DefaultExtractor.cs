// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Generic;
using wyvern.api.abstractions;

namespace wyvern.utils
{
    /// <summary>
    /// Extracts properties of a message into the given dictionary
    /// </summary>
    /// <remarks>
    /// Note: by default this does nothing
    /// </remarks>
    public class DefaultExtractor : IMessagePropertyExtractor
    {
        public Dictionary<string, object> Extract<T>(T obj)
        {
            return new Dictionary<string, object>();
        }
    }

}