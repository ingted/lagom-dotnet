// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api
{
    public interface IHeaderFilter
    {
        RequestHeader TransformClientRequest(RequestHeader request);
        RequestHeader TransformServerRequest(RequestHeader request);
    }
}