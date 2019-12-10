// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Generic;

namespace wyvern.api.abstractions
{
    public interface IMessagePropertyExtractor
    {
        Dictionary<string, object> Extract<T>(T obj);
    }
}