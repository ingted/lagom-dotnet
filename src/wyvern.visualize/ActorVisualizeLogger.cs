// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using Akka.Actor;

namespace wyvern.visualize
{
    public class ActorVisualizeLogger : ReceiveActor
    {
        public const string Name = "AkkaVisualizer";

        protected override void PreStart()
        {
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
        }

    }
}
