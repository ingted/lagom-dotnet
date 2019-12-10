// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with 404 Not Found
    /// </summary>
    public sealed class NotFoundException : StatusCodeException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
