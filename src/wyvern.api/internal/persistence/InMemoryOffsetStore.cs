// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using wyvern.api.abstractions;

namespace wyvern.api.@internal.persistence
{
    /// <summary>
    /// Offset Store for local development, not to be used in production
    /// </summary>
    [Obsolete("Not for production use")]
    internal class InMemoryOffsetStore : IOffsetStore
    {
        public Task<IOffsetDao> Prepare(string processorId, string tag)
        {
            return Task.FromResult<IOffsetDao>(new InMemoryOffsetDao());
        }
    }

}
