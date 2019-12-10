// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


﻿using Akka.Util;
using wyvern.visualize.Clients;

namespace wyvern.visualize
{
    internal class VisualizeRegistry
    {
        private readonly ConcurrentSet<IActorVisualizeClient> _activeClients = new ConcurrentSet<IActorVisualizeClient>();

        public bool AddMonitor(IActorVisualizeClient client)
        {
            return _activeClients.TryAdd(client);
        }

        public bool RemoveMonitor(IActorVisualizeClient client)
        {
            return _activeClients.TryRemove(client);
        }
    }
}
