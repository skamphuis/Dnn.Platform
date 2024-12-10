// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline
{
    using System.Web.Mvc;

    using DotNetNuke.Common;
    using DotNetNuke.DependencyInjection;
    using DotNetNuke.Web.Mvc.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class Startup : IDnnStartup
    {
        /// <inheritdoc/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcControllers();

            DependencyResolver.SetResolver(new DnnMvcPipelineDependencyResolver(Globals.DependencyProvider));
        }
    }
}
