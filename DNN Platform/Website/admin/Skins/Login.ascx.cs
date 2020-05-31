﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using System;
using System.Web;
using Microsoft.Extensions.DependencyInjection;

using DotNetNuke.Common;
using DotNetNuke.Abstractions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.Skins.Controls
{
    /// -----------------------------------------------------------------------------
    /// <summary></summary>
    /// <remarks></remarks>
    /// -----------------------------------------------------------------------------
    public partial class Login : SkinObjectBase
    {
        private readonly INavigationManager _navigationManager;
        public Login()
        {
            _navigationManager = Globals.DependencyProvider.GetRequiredService<INavigationManager>();
            LegacyMode = true;
        }

		#region Private Members

        private const string MyFileName = "Login.ascx";
		#endregion

		#region Public Members

        public string Text { get; set; }

        public string CssClass { get; set; }

        public string LogoffText { get; set; }

        /// <summary>
        /// Set this to false in the skin to take advantage of the enhanced markup
        /// </summary>
        public bool LegacyMode { get; set; }

        /// <summary>
        /// set this to true to show in custom 404/500 page.
        /// </summary>
        public bool ShowInErrorPage { get; set; }

		#endregion

		#region Event Handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Visible = (!PortalSettings.HideLoginControl || Request.IsAuthenticated)
		                && (!PortalSettings.InErrorPageRequest() || ShowInErrorPage);
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

	        if (Visible)
	        {
		        try
		        {
			        if (LegacyMode)
			        {
				        loginLink.Visible = true;
				        loginGroup.Visible = false;
			        }
			        else
			        {
				        loginLink.Visible = false;
				        loginGroup.Visible = true;
			        }

			        if (!String.IsNullOrEmpty(CssClass))
			        {
				        loginLink.CssClass = CssClass;
				        enhancedLoginLink.CssClass = CssClass;
			        }

			        if (Request.IsAuthenticated)
			        {
				        if (!String.IsNullOrEmpty(LogoffText))
				        {
					        if (LogoffText.IndexOf("src=") != -1)
					        {
						        LogoffText = LogoffText.Replace("src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath);
					        }
					        loginLink.Text = LogoffText;
					        enhancedLoginLink.Text = LogoffText;
				        }
				        else
				        {
					        loginLink.Text = Localization.GetString("Logout", Localization.GetResourceFile(this, MyFileName));
					        enhancedLoginLink.Text = loginLink.Text;
					        loginLink.ToolTip = loginLink.Text;
					        enhancedLoginLink.ToolTip = loginLink.Text;
				        }
				        loginLink.NavigateUrl = _navigationManager.NavigateURL(PortalSettings.ActiveTab.TabID, "Logoff");
				        enhancedLoginLink.NavigateUrl = loginLink.NavigateUrl;
			        }
			        else
			        {
				        if (!String.IsNullOrEmpty(Text))
				        {
					        if (Text.IndexOf("src=") != -1)
					        {
						        Text = Text.Replace("src=\"", "src=\"" + PortalSettings.ActiveTab.SkinPath);
					        }
					        loginLink.Text = Text;
					        enhancedLoginLink.Text = Text;
				        }
				        else
				        {
					        loginLink.Text = Localization.GetString("Login", Localization.GetResourceFile(this, MyFileName));
					        enhancedLoginLink.Text = loginLink.Text;
					        loginLink.ToolTip = loginLink.Text;
					        enhancedLoginLink.ToolTip = loginLink.Text;
				        }

				        string returnUrl = HttpContext.Current.Request.RawUrl;
				        if (returnUrl.IndexOf("?returnurl=") != -1)
				        {
					        returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="));
				        }
				        returnUrl = HttpUtility.UrlEncode(returnUrl);

				        loginLink.NavigateUrl = Globals.LoginURL(returnUrl, (Request.QueryString["override"] != null));
				        enhancedLoginLink.NavigateUrl = loginLink.NavigateUrl;

                        //avoid issues caused by multiple clicks of login link
                        var oneclick = "this.disabled=true;";
			            if (Request.UserAgent != null && Request.UserAgent.Contains("MSIE 8.0")==false)
			            {
                            loginLink.Attributes.Add("onclick", oneclick);
                            enhancedLoginLink.Attributes.Add("onclick", oneclick);
			            }

				        if (PortalSettings.EnablePopUps && PortalSettings.LoginTabId == Null.NullInteger && !AuthenticationController.HasSocialAuthenticationEnabled(this))
				        {
					        //To avoid duplicated encodes of URL
                            var clickEvent = "return " + UrlUtils.PopUpUrl(HttpUtility.UrlDecode(loginLink.NavigateUrl), this, PortalSettings, true, false, 300, 650);
					        loginLink.Attributes.Add("onclick", clickEvent);
					        enhancedLoginLink.Attributes.Add("onclick", clickEvent);
				        }
			        }
		        }
		        catch (Exception exc)
		        {
			        Exceptions.ProcessModuleLoadException(this, exc);
		        }
	        }
        }

		#endregion
    }
}
