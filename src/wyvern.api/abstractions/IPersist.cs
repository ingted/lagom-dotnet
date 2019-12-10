// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.entity.@event;

namespace wyvern.api.abstractions
{
    /// <summary>
    /// Persistence interface
    /// </summary>
    /// <typeparam name="TE"></typeparam>
    public interface IPersist<out TE>
        where TE : AbstractEvent
    {
    }
}
