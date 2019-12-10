// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with 401 Unauthorized
    /// </summary>
    public sealed class UnauthorizedException : StatusCodeException
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
