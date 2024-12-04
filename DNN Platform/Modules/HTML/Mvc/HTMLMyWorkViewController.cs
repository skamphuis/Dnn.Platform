// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Modules.Html.Mvc
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
    using DotNetNuke.Mvc;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client.ClientResourceManagement;
    using DotNetNuke.Web.Mvc;
    using DotNetNuke.Web.Mvc.Page;
    using DotNetNuke.Website.Controllers;
    using DotNetNuke.Website.Models;
    using Microsoft.Extensions.DependencyInjection;

    public class HTMLMyWorkViewController : ModuleControllerBase
    {
        private readonly INavigationManager navigationManager;

        public HTMLMyWorkViewController()
        {
            this.navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
        }

        [ChildActionOnly]
        public ActionResult Invoke(ModuleInfo module)
        {
            var objHtmlTextUsers = new HtmlTextUserController();
            var lst = objHtmlTextUsers.GetHtmlTextUser(this.UserInfo.UserID).Cast<HtmlTextUserInfo>();
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/DesktopModules/HTML/edit.css");
            MvcClientResourceManager.RegisterStyleSheet(this.ControllerContext, "~/Portals/_default/Skins/_default/WebControlSkin/Default/GridView.default.css");
            return this.View(module, new MyWorkModel()
            {
                LocalResourceFile = Path.Combine(Path.GetDirectoryName(module.ModuleControl.ControlSrc), Localization.LocalResourceDirectory + "/" + Path.GetFileNameWithoutExtension(module.ModuleControl.ControlSrc)),
                ModuleId = module.ModuleID,
                TabId = module.TabID,
                RedirectUrl = this.navigationManager.NavigateURL(),
                HtmlTextUsers = lst.Select(u => new HtmlTextUserModel()
                {
                    Url = this.navigationManager.NavigateURL(u.TabID),
                    ModuleID = u.ModuleID,
                    ModuleTitle = u.ModuleTitle,
                    StateName = u.StateName,
                }).ToList(),
            });
        }
    }
}
