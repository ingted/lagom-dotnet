// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using wyvern.api.abstractions;

namespace wyvern.api.@internal.persistence
{
    /// <summary>
    /// Offset DAO for local development, not to be used in production
    /// </summary>
    [Obsolete("Not for production use")]
    internal class InMemoryOffsetDao : IOffsetDao
    {
        public Offset LoadedOffset { get; private set; } = Offset.NoOffset();

        public Task<Done> SaveOffset(Offset o)
        {
            LoadedOffset = o;
            return Task.FromResult(Done.Instance);
        }
    }
}
