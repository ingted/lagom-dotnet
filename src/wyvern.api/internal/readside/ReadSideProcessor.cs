// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.@internal.readside
{
    public abstract class ReadSideProcessor<TE>
        where TE : AggregateEvent<TE>
    {
        public IConfiguration Config { get; internal set; }
        public Config Config2 { get; internal set; }
        public abstract AggregateEventTag[] AggregateTags { get; }
        public string ReadSideName => GetType().Name;
        public abstract ReadSideHandler<TE> BuildHandler();
    }
}