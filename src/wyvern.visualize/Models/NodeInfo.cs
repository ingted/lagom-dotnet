// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.visualize.Models
{
    public class NodeInfo
    {
        // full path
        public string Path { get; set; }
        // last name path
        public string Name { get; set; }

        // ClassType
        public string Type { get; set; }
        /// <summary>
        /// Short type name
        /// </summary>
        public string TypeName { get; set; }

        public bool IsLocal { get; set; }

        public bool IsTerminated { get; set; }

        public int NoOfMessages { get; set; }

        public RouterInfo Router { get; set; }
    }

    public class ActorSystemInfo : NodeInfo
    {

    }
}
