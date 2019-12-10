// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Threading.Tasks;

namespace wyvern.api.abstractions
{
    public interface IShardedEntityReference
    {
        Task<TR> Ask<TR>(IReplyType<TR> command);
    }
}
