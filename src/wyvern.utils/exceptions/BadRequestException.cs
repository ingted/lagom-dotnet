// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with 400 Bad Request
    /// </summary>
    public sealed class BadRequestException : StatusCodeException
    {
        public BadRequestException(string message) : base(message) { }
    }
}
