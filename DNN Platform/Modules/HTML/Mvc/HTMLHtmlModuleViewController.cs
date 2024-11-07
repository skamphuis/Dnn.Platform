// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Framework.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Abstractions;
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Content.Workflow.Entities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Settings;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Modules.Html;
    using DotNetNuke.Modules.Html.Components;
    using DotNetNuke.Modules.Html.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc;
    using DotNetNuke.Web.Mvc.Page;
    using DotNetNuke.Website.Controllers;
    using DotNetNuke.Website.Models;
    using Microsoft.Extensions.DependencyInjection;

    public class HTMLHtmlModuleViewController : ModuleSettingsController
    {
        private readonly INavigationManager navigationManager;
        private readonly HtmlTextController htmlTextController;

        public HTMLHtmlModuleViewController()
        {
            this.navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            this.htmlTextController = new HtmlTextController(this.navigationManager);
        }

        [ChildActionOnly]
        public ActionResult Invoke(ModuleInfo module)
        {
            // ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            int workflowID = this.htmlTextController.GetWorkflow(module.ModuleID, module.TabID, module.PortalID).Value;

            HtmlTextInfo content = this.htmlTextController.GetTopHtmlText(module.ModuleID, true, workflowID);
            var html = string.Empty;
            if (content != null)
            {
                html = System.Web.HttpUtility.HtmlDecode(content.Content);
            }

            return this.View(module, new HtmlModuleModel()
            {
                Html = html,
            });
        }
    }
}
