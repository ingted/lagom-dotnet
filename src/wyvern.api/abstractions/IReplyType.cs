// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.abstractions
{
    /// <summary>
    /// Typed reply type interface
    /// </summary>
    /// <remarks>
    /// When used in the context of an `AbstractCommand` it binds the command to the
    /// `TR` type of response.
    /// </remarks>
    /// <typeparam name="TR"></typeparam>
    public interface IReplyType<TR> : IReplyType { }

    /// <summary>
    /// Masked reply type interface
    /// </summary>
    public interface IReplyType { }
}