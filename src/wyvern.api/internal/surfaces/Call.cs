// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using wyvern.api.abstractions;

namespace wyvern.api.@internal.surfaces
{
    /// <summary>
    /// Represents a specific, untyped call
    /// </summary>
    internal abstract class Call : ICall
    {
        public ICallId CallId { get; }
        public Delegate MethodRef { get; }

        protected Call(CallId callId, Delegate methodRef)
        {
            (CallId, MethodRef) =
                (callId, methodRef);
        }
    }

    /// <summary>
    /// Represents a specific call given a response and request pair
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal sealed class Call<TRequest, TResponse> : Call, ICall<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callId">Call identifier</param>
        /// <param name="methodRef">Method reference to service call</param>
        public Call(CallId callId, Delegate methodRef)
            : base(callId, methodRef)
        {
        }
    }
}
