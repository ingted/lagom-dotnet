// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with 401 Forbidden
    /// </summary>
    public sealed class ForbiddenException : StatusCodeException
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
