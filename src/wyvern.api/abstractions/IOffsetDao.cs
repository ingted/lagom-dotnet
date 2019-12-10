// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Threading.Tasks;
using Akka;
using Akka.Persistence.Query;

namespace wyvern.api.abstractions
{
    public interface IOffsetDao
    {
        Offset LoadedOffset { get; }
        Task<Done> SaveOffset(Offset o);
    }
}
