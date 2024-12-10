// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.MvcPipeline
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Framework.JavaScriptLibraries;

    // using DotNetNuke.Framework.Models;
    using DotNetNuke.Mvc;
    using DotNetNuke.UI.Modules;
    using DotNetNuke.Web.Client.ClientResourceManagement;

    public static partial class HtmlHelpers
    {
        public static IHtmlString ViewComponent(this HtmlHelper htmlHelper, string controllerName, object model)
        {
            return htmlHelper.Action("Invoke", controllerName, model);
        }

        public static IHtmlString Control(this HtmlHelper htmlHelper, string controlSrc, object model)
        {
            try
            {
                return htmlHelper.Action("Invoke", MvcUtils.GetControlControllerName(controlSrc), model);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {MvcUtils.GetControlControllerName(controlSrc)} - Invoke", ex);
            }
        }

        public static IHtmlString Control(this HtmlHelper htmlHelper, ModuleInfo module)
        {
            try
            {
                return htmlHelper.Action("Invoke", MvcUtils.GetControlControllerName(module.ModuleControl.ControlSrc), module);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {MvcUtils.GetControlControllerName(module.ModuleControl.ControlSrc)} - Invoke", ex);
            }
        }

        public static IHtmlString CspNonce(this HtmlHelper htmlHelper)
        {
            return new MvcHtmlString(htmlHelper.ViewContext.HttpContext.Items["CSP-NONCE"].ToString());
        }
    }
}
