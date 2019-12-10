// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Akka.Actor;
using Akka.Persistence.Query.Sql;
using Akka.Util;

namespace wyvern.api.@internal.sharding
{
    internal class SqlServerShardedEntityRegistry : ShardedEntityRegistry
    {
        internal SqlServerShardedEntityRegistry(ActorSystem system)
            : base(system)
        {
        }

        protected override Option<string> QueryPluginId { get; }
            = new Option<string>(SqlReadJournal.Identifier);
    }
}
