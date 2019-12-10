// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


﻿using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;
using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    internal class SqlServerOffsetDao : IOffsetDao
    {
        SqlServerOffsetStore Store { get; }

        string ReadSideId { get; }
        string Tag { get; }

        public Offset LoadedOffset { get; }

        public SqlServerOffsetDao(SqlServerOffsetStore store, string readSideId, string tag, Offset loadedOffset)
        {
            Store = store;
            ReadSideId = readSideId;
            Tag = tag;
            LoadedOffset = loadedOffset;
        }

        public Task<Done> SaveOffset(Offset o)
        {
            return Store.UpdateOffset(ReadSideId, Tag, o);
        }
    }

}
