// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Threading.Tasks;
using wyvern.visualize.Clients;
using wyvern.visualize.Models;

namespace wyvern.visualize
{
    public class WebApiVisualizer : IActorVisualizeClient
    {
        private IActorVisualizer _actorVisualizer;

        public static WebApiVisualizer Root { get; internal set; }

        public WebApiVisualizer()
        {
            Root = this;
        }

        public void SetVisualizer(IActorVisualizer actorVisualizer)
        {
            _actorVisualizer = actorVisualizer;
        }

        public async Task<QueryResult> List(string path)
        {
            return await _actorVisualizer.List(path);
        }

        public Task<NodeInfo> Send(string path, string messageType)
        {
            return _actorVisualizer.Send(path, messageType);
        }

        public void Dispose()
        {

        }
    }
}
