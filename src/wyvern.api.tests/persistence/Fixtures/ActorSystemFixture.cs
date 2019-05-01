using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit.Internals;
using Akka.TestKit;

namespace wyvern.api.tests
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