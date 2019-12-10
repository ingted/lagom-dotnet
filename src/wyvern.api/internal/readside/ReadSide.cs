// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.@internal.readside
{
    public abstract class ReadSide
    {
        public abstract void Register<TE>(
            Func<ReadSideProcessor<TE>> processorFactory
        ) where TE : AggregateEvent<TE>;
    }
}
