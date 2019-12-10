// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Immutable;
using wyvern.utils;

namespace wyvern.api.abstractions
{
    /// <summary>
    /// Descriptor interface
    /// </summary>
    [Immutable]
    public interface IDescriptor
    {
        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        string Name { get; }

        /// <summary>
        /// Header filter
        /// </summary>
        IHeaderFilter HeaderFilter { get; }

        // TODO: ExceptionSerializer

        /// <summary>
        /// Calls
        /// </summary>
        /// <value></value>
        ImmutableArray<ICall> Calls { get; }

        /// <summary>
        /// Topics
        /// </summary>
        /// <value></value>
        ImmutableArray<ITopicCall> Topics { get; }

        /// <summary>
        /// Add calls to the descriptor
        /// </summary>
        /// <param name="calls"></param>
        /// <returns></returns>
        IDescriptor WithCalls(params ICall[] calls);

        /// <summary>
        /// Add topics to the descriptor
        /// </summary>
        /// <param name="topics"></param>
        /// <returns></returns>
        IDescriptor WithTopics(params ITopicCall[] topics);

        /// <summary>
        /// Apply filters to the headers
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IDescriptor WithHeaderFilter(IHeaderFilter filter);
    }
}
