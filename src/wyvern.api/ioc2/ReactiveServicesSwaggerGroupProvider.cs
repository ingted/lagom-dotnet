// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace wyvern.api.ioc
{
    /// <summary>
    /// Part of the swagger document generation pipeline which deals with description groups
    /// </summary>
    internal class ReactiveServicesSwaggerGroupProvider : IApiDescriptionGroupCollectionProvider
    {
        public ApiDescriptionGroupCollection ApiDescriptionGroups { get; } =
            new ApiDescriptionGroupCollection(
                // Note: registering groups and descriptions within here would
                // require us to re-iterate from within the custom document
                // filter.
                new List<ApiDescriptionGroup>(), 1
            );
    }
}
