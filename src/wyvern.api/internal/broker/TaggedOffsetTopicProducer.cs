// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Amqp;
using wyvern.api.abstractions;
using wyvern.api.@internal.persistence;
using wyvern.api.@internal.surfaces;
using wyvern.entity.@event;
using wyvern.entity.@event.aggregate;
using static wyvern.api.@internal.broker.Producer;
using static wyvern.api.@internal.persistence.SqlServerOffsetStore;

namespace wyvern.api.@internal.broker
{
    /// <summary>
    /// Topic producer
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    internal sealed class TaggedOffsetTopicProducer<TEvent> :
        InternalTopic<TEvent>,
        ITaggedOffsetTopicProducer<TEvent>
        where TEvent : AbstractEvent
    {
        /// <summary>
        /// Event source factory
        /// </summary>
        /// <value></value>
        public Func<AggregateEventTag, Offset, Source<KeyValuePair<TEvent, Offset>, NotUsed>> ReadSideStream { get; }

        /// <summary>
        /// Set of tags
        /// </summary>
        /// <value></value>
        public ImmutableArray<AggregateEventTag> Tags { get; }

        /// <summary>
        /// Amqp sender link
        /// </summary>
        /// <value></value>
        SenderLink SenderLink { get; }

        public TaggedOffsetTopicProducer(
            ImmutableArray<AggregateEventTag> tags,
            Func<AggregateEventTag, Offset, Source<KeyValuePair<TEvent, Offset>, NotUsed>> readSideStream)
        {
            (Tags, ReadSideStream) = (tags, readSideStream);
        }

        public void Init(ActorSystem sys, string topicId, ISerializer serializer, IMessagePropertyExtractor extractor)
        {
            var config = sys.Settings.Config;
            foreach (var tag in Tags)
                Producer.StartTaggedOffsetProducer<TEvent>(
                    sys,
                    Tags,
                    new TopicConfig(config),
                    topicId,
                    (string entityId, Offset o) => ReadSideStream.Invoke(tag, o),
                    serializer,
                    extractor,
                    new SqlServerOffsetStore(
                        new SqlServerProvider(config).GetconnectionProvider(),
                        new OffsetStoreConfiguration(sys.Settings.Config)
                    )
                );

        }

    }
}