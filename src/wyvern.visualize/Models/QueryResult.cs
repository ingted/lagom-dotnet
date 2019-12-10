// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


﻿using System.Collections.Generic;

namespace wyvern.visualize.Models
{
    public class QueryResult
    {
        public QueryResult(string path, List<NodeInfo> children)
        {
            Path = path;
            Children = children;
        }

        public string Path { get; }
        public List<NodeInfo> Children { get; }
    }
}
