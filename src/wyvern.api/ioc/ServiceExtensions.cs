// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Text;
using Akka.Actor;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using wyvern.api.abstractions;
using wyvern.api.@internal.surfaces;
using wyvern.visualize;

namespace wyvern.api.ioc
{
    public static partial class ServiceExtensions
    {
        /// <summary>
        /// Interface for probing route collection functionality
        /// </summary>
        public interface IRouteCollectionConsumer
        {
            void Consume(RouteCollection collection);
        }

        /// <summary>
        /// Add a cluster visualizer to the endpoints /api/visualizer/list and
        /// /api/visualizer/send
        /// </summary>
        /// <param name="router"></param>
        private static void AddVisualizer(IRouteBuilder router)
        {
            router.MapGet("/api/visualizer/list", async (req, res, ctx) =>
            {
                req.Query.TryGetValue("path", out var path);
                var obj = await WebApiVisualizer.Root.List(path);
                var jsonString = JsonConvert.SerializeObject(obj);
                byte[] content = Encoding.UTF8.GetBytes(jsonString);
                res.ContentType = "application/json";
                await res.Body.WriteAsync(content, 0, content.Length);
            });

            router.MapGet("/api/visualizer/send", async (req, res, ctx) =>
            {
                req.Query.TryGetValue("path", out var path);
                req.Query.TryGetValue("messageType", out var messageType);
                var obj = await WebApiVisualizer.Root.Send(path, messageType);
                var jsonString = JsonConvert.SerializeObject(obj);
                byte[] content = Encoding.UTF8.GetBytes(jsonString);
                res.ContentType = "application/json";
                await res.Body.WriteAsync(content, 0, content.Length);
            });
        }

        private static void RegisterTopic(object t, Service s, ISerializer serializer, IMessagePropertyExtractor extractor, ActorSystem sys)
        {
            var topicCall = (ITopicCall)t;
            if (!(topicCall.TopicHolder is MethodTopicHolder))
                throw new NotImplementedException();

            var producer = ((MethodTopicHolder)topicCall.TopicHolder).Method.Invoke(s, null);
            typeof(ITaggedOffsetTopicProducer<>)
            .MakeGenericType(producer.GetType().GetGenericArguments()[0])
                .GetMethod("Init")
                .Invoke(producer, new object[] { sys, topicCall.TopicId.Name, serializer, extractor });

        }


    }
}