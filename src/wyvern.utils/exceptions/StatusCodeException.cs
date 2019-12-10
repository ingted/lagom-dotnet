// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.api.exceptions
{
    /// <summary>
    /// Exception with HTTP status code
    /// </summary>
    public abstract class StatusCodeException : Exception
    {
        public StatusCodeException(string message) : base(message) { }
    }
}
