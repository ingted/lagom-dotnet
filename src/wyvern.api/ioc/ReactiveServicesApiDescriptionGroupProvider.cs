﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace wyvern.api.ioc
{
    /// <summary>
    /// Part of the swagger document generation pipeline which deals with description groups
    /// </summary>
    internal class ReactiveServicesApiDescriptionGroupProvider : IApiDescriptionGroupCollectionProvider
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