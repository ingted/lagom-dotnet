// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.utils.exceptions
{
    /// <summary>
    /// Exception with 409 Conflict
    /// /// </summary>
    public sealed class ConflictException : StatusCodeException
    {
        public ConflictException(string message) : base(message) { }
    }
}
