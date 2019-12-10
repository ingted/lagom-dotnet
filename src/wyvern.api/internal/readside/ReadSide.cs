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
        public abstract void Register<T, TE>(Func<T> processorFactory = null)
            where T : ReadSideProcessor<TE>
            where TE : AggregateEvent<TE>;
    }
}
