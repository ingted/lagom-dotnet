// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.visualize.Clients
{
    /// <summary>
    /// Interfce for clients wanting to interact with the visualizer
    /// </summary>
    public interface IActorVisualizeClient : IDisposable
    {

        void SetVisualizer(IActorVisualizer actorVisualizer);
    }
}
