// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.api.abstractions
{
    /// <summary>
    /// Untyped ICall interface
    /// </summary>
    public interface ICall
    {
        ICallId CallId { get; }
        Delegate MethodRef { get; }
    }

    /// <summary>
    /// Interface supplied to support covariance/contravarience
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICall<in TRequest, out TResponse> : ICall { }
}