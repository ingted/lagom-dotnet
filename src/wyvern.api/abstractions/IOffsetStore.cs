// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Threading.Tasks;

namespace wyvern.api.abstractions
{
    public interface IOffsetStore
    {
        Task<IOffsetDao> Prepare(string processorId, string tag);
    }
}
