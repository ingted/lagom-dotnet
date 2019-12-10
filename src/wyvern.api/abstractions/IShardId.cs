// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.abstractions
{
    /// <summary>
    /// Shard identifier
    /// </summary>
    public interface IShardId
    {
        /// <summary>
        /// Shard identifier
        /// </summary>
        /// <value></value>
        string ShardId { get; }
    }
}
