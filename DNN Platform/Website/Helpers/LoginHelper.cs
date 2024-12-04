// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Helpers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Services.Authentication;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Entities.Portals;

    public static class LoginHelper
    {
        private const string MyFileName = "Login.ascx";

        public static MvcHtmlString Login(
            this HtmlHelper helper,
            string text = "",
            string cssClass = "",
            string logoffText = "",
            bool legacyMode = true,
            bool showInErrorPage = false)
        {
            var portalSettings = PortalSettings.Current;
            var request = HttpContext.Current.Request;

            var isVisible = (!portalSettings.HideLoginControl || request.IsAuthenticated)
                        && (!portalSettings.InErrorPageRequest() || showInErrorPage);

            if (!isVisible)
            {
                return MvcHtmlString.Empty;
            }

            if (legacyMode)
            {
                return BuildLegacyLogin(text, cssClass, logoffText);
            }

            return BuildEnhancedLogin(text, cssClass, logoffText);
        }

        private static MvcHtmlString BuildLegacyLogin(string text, string cssClass, string logoffText)
        {
            var link = new TagBuilder("a");
            ConfigureLoginLink(link, text, cssClass, logoffText);
            return new MvcHtmlString(link.ToString() + GetLoginScript());
        }

        private static MvcHtmlString BuildEnhancedLogin(string text, string cssClass, string logoffText)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("loginGroup");

            var link = new TagBuilder("a");
            link.AddCssClass("secondaryActionsList");
            ConfigureLoginLink(link, text, cssClass, logoffText);

            container.InnerHtml = link.ToString();
            return new MvcHtmlString(container.ToString() + GetLoginScript());
        }

        private static void ConfigureLoginLink(TagBuilder link, string text, string cssClass, string logoffText)
        {
            var portalSettings = PortalSettings.Current;
            var request = HttpContext.Current.Request;
            var navigationManager = Globals.DependencyProvider.GetRequiredService<DotNetNuke.Abstractions.INavigationManager>();

            if (!string.IsNullOrEmpty(cssClass))
            {
                link.AddCssClass(cssClass);
            }
            else
            {
                link.AddCssClass("SkinObject");
            }

            link.Attributes["rel"] = "nofollow";

            if (request.IsAuthenticated)
            {
                var displayText = !string.IsNullOrEmpty(logoffText) 
                    ? logoffText.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath)
                    : Localization.GetString("Logout", Localization.GetResourceFile(null, MyFileName));

                link.SetInnerText(displayText);
                link.Attributes["title"] = displayText;
                link.Attributes["href"] = navigationManager.NavigateURL(portalSettings.ActiveTab.TabID, "Logoff");
            }
            else
            {
                var displayText = !string.IsNullOrEmpty(text)
                    ? text.Replace("src=\"", "src=\"" + portalSettings.ActiveTab.SkinPath)
                    : Localization.GetString("Login", Localization.GetResourceFile(null, MyFileName));

                link.SetInnerText(displayText);
                link.Attributes["title"] = displayText;

                string returnUrl = request.RawUrl;
                if (returnUrl.IndexOf("?returnurl=", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl=", StringComparison.OrdinalIgnoreCase));
                }
                returnUrl = HttpUtility.UrlEncode(returnUrl);

                var loginUrl = Globals.LoginURL(returnUrl, request.QueryString["override"] != null);
                link.Attributes["href"] = loginUrl;
                link.Attributes["data-url"] = loginUrl;
                link.Attributes["class"] += " dnnLoginLink";
            }
        }

        private static string GetLoginScript()
        {
            var portalSettings = PortalSettings.Current;
            var request = HttpContext.Current.Request;

            if (!request.IsAuthenticated)
            {
                var script = @"
                    <script>
                    $(function() {
                        var $loginLink = $('.dnnLoginLink');
                        if ($loginLink.length > 0) {
                            $loginLink.on('click', function(e) {
                                e.preventDefault();
                                var $this = $(this);
                                var url = $this.data('url');
                                
                                if (!navigator.userAgent.match(/MSIE 8.0/)) {
                                    $this.prop('disabled', true);
                                }
                                ";

                if (portalSettings.EnablePopUps && 
                    portalSettings.LoginTabId == Null.NullInteger && 
                    !AuthenticationController.HasSocialAuthenticationEnabled(null))
                {
                    script += string.Format(@"
                        var popupUrl = {0};
                        window.location = popupUrl;
                        ", 
                        UrlUtils.PopUpUrl("' + url + '", null, portalSettings, true, false, 300, 650)
                    );
                }
                else 
                {
                    script += "window.location = url;";
                }

                script += @"
                            });
                        }
                    });
                    </script>";

                return script;
            }

            return string.Empty;
        }
    }
} 