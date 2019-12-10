// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Reflection;

namespace wyvern.api.@internal.surfaces
{
    internal class MethodTopicHolder : TopicHolder
    {
        public MethodTopicHolder(MethodInfo method)
        {
            Method = method;
        }

        public MethodInfo Method { get; }

        public Topic<M> Create<M>(Service service)
        {
            return (Topic<M>)Method.Invoke(service, null);
        }
    }
}