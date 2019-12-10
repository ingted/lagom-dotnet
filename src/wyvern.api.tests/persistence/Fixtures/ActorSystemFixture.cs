// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using Akka.Actor;
using Akka.Configuration;

namespace wyvern.api.tests.persistence.Fixtures
{
    public class ActorSystemFixture : IDisposable
    {
        public ActorSystem ActorSystem { get; }

        public ActorSystemFixture()
        {
            ActorSystem = ActorSystem.Create("ClusterSystem", ConfigurationFactory.ParseString(@"
akka {
    actor {
        provider = cluster
    }
    cluster {
        seed-nodes = [""akka.tcp://ClusterSystem@localhost:7000""]
        roles = [ ""default"" ]
    }
    loglevel = INFO
    remote {
        dot-netty.tcp {
            hostname = 127.0.0.1
            port = 7000
            public-hostname = ""localhost""
        }
    }
}
wyvern.cluster {}
wyvern.persistence {}"));
        }

        public void Dispose()
        {
            ActorSystem.Terminate();
            ActorSystem.WhenTerminated.Wait();
        }
    }
}