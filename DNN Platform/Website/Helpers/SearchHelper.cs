// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Skins
{
    using System;
    using System.Web;
    using System.Web.Mvc;

    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Host;
    using DotNetNuke.Entities.Icons;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.Client;
    using DotNetNuke.Web.Client.ClientResourceManagement;

    public static partial class SkinHelpers
    {
        private const string MyFileName = "Search.ascx";

        public static MvcHtmlString Search2(
            this HtmlHelper helper,
            bool useDropDownList = false,
            bool showWeb = true,
            bool showSite = true,
            string cssClass = "",
            string submit = null,
            string webText = null,
            string siteText = null,
            bool enableWildSearch = true,
            int minCharRequired = 2,
            int autoSearchDelayInMilliSecond = 400)
        {
            Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            MvcClientResourceManager.RegisterStyleSheet(helper.ViewContext, "~/Resources/Search/SearchSkinObjectPreview.css", FileOrder.Css.ModuleCss);
            MvcClientResourceManager.RegisterScript(helper.ViewContext, "~/Resources/Search/SearchSkinObjectPreview.js");

            if (!useDropDownList)
            {
                return BuildClassicSearch(showWeb, showSite, cssClass, submit, webText, siteText, enableWildSearch, minCharRequired, autoSearchDelayInMilliSecond);
            }

            return BuildDropDownSearch(cssClass, submit, webText, siteText, enableWildSearch, minCharRequired, autoSearchDelayInMilliSecond);
        }

        private static MvcHtmlString BuildClassicSearch(
            bool showWeb,
            bool showSite,
            string cssClass,
            string submit,
            string webText,
            string siteText,
            bool enableWildSearch,
            int minCharRequired,
            int autoSearchDelayInMilliSecond)
        {
            var container = new TagBuilder("span");
            container.GenerateId("ClassicSearch");

            if (showWeb)
            {
                var radio = new TagBuilder("input");
                radio.Attributes["type"] = "radio";
                radio.Attributes["name"] = "SearchType";
                radio.Attributes["value"] = "W";
                radio.Attributes["id"] = "WebRadioButton";
                radio.Attributes["class"] = cssClass;
                radio.Attributes["checked"] = "checked";
                container.InnerHtml += radio.ToString(TagRenderMode.SelfClosing);

                var label = new TagBuilder("label");
                label.Attributes["for"] = "WebRadioButton";
                label.SetInnerText(webText ?? Localization.GetString("Web", GetSkinsResourceFile(MyFileName)));
                container.InnerHtml += label.ToString();
            }

            if (showSite)
            {
                var radio = new TagBuilder("input");
                radio.Attributes["type"] = "radio";
                radio.Attributes["name"] = "SearchType";
                radio.Attributes["value"] = "S";
                radio.Attributes["id"] = "SiteRadioButton";
                radio.Attributes["class"] = cssClass;
                container.InnerHtml += radio.ToString(TagRenderMode.SelfClosing);

                var label = new TagBuilder("label");
                label.Attributes["for"] = "SiteRadioButton";
                label.SetInnerText(siteText ?? Localization.GetString("Site", GetSkinsResourceFile(MyFileName)));
                container.InnerHtml += label.ToString();
            }

            container.InnerHtml += BuildSearchInput("txtSearch", "NormalTextBox");
            container.InnerHtml += BuildSearchButton(cssClass, submit);

            return new MvcHtmlString(container.ToString() + GetInitScript(false, enableWildSearch, minCharRequired, autoSearchDelayInMilliSecond));
        }

        private static MvcHtmlString BuildDropDownSearch(
            string cssClass,
            string submit,
            string webText,
            string siteText,
            bool enableWildSearch,
            int minCharRequired,
            int autoSearchDelayInMilliSecond)
        {
            var container = new TagBuilder("div");
            container.GenerateId("DropDownSearch");
            container.AddCssClass("SearchContainer");

            var searchBorder = new TagBuilder("div");
            searchBorder.AddCssClass("SearchBorder");

            var searchIcon = new TagBuilder("div");
            searchIcon.GenerateId("SearchIcon");
            searchIcon.AddCssClass("SearchIcon");

            var img = new TagBuilder("img");
            img.Attributes["src"] = IconController.IconURL("Action");
            img.Attributes["alt"] = Localization.GetString("DropDownGlyph.AltText", GetSkinsResourceFile(MyFileName));
            searchIcon.InnerHtml = img.ToString(TagRenderMode.SelfClosing);

            searchBorder.InnerHtml += searchIcon.ToString();
            searchBorder.InnerHtml += BuildSearchInput("txtSearchNew", "SearchTextBox");

            var choices = new TagBuilder("ul");
            choices.GenerateId("SearchChoices");

            var siteLi = new TagBuilder("li");
            siteLi.GenerateId("SearchIconSite");
            siteLi.SetInnerText(siteText ?? Localization.GetString("Site", GetSkinsResourceFile(MyFileName)));
            choices.InnerHtml += siteLi.ToString();

            var webLi = new TagBuilder("li");
            webLi.GenerateId("SearchIconWeb");
            webLi.SetInnerText(webText ?? Localization.GetString("Web", GetSkinsResourceFile(MyFileName)));
            choices.InnerHtml += webLi.ToString();

            searchBorder.InnerHtml += choices.ToString();
            container.InnerHtml = searchBorder.ToString() + BuildSearchButton(cssClass, submit);

            return new MvcHtmlString(container.ToString() + GetInitScript(true, enableWildSearch, minCharRequired, autoSearchDelayInMilliSecond));
        }

        private static string BuildSearchInput(string id, string cssClass)
        {
            var container = new TagBuilder("span");
            container.AddCssClass("searchInputContainer");
            container.Attributes["data-moreresults"] = GetSeeMoreText();
            container.Attributes["data-noresult"] = GetNoResultText();

            var input = new TagBuilder("input");
            input.Attributes["type"] = "text";
            input.Attributes["id"] = id;
            input.Attributes["class"] = cssClass;
            input.Attributes["maxlength"] = "255";
            input.Attributes["autocomplete"] = "off";
            input.Attributes["placeholder"] = GetPlaceholderText();
            input.Attributes["aria-label"] = "Search";

            var clear = new TagBuilder("a");
            clear.AddCssClass("dnnSearchBoxClearText");
            clear.Attributes["title"] = GetClearQueryText();

            container.InnerHtml = input.ToString(TagRenderMode.SelfClosing) + clear.ToString();
            return container.ToString();
        }

        private static string BuildSearchButton(string cssClass, string submit)
        {
            var button = new TagBuilder("a");
            button.AddCssClass("SearchButton " + cssClass);
            button.Attributes["href"] = "#";
            button.InnerHtml = submit ?? Localization.GetString("Search", GetSkinsResourceFile(MyFileName));
            return button.ToString();
        }

        private static string GetInitScript(bool useDropDownList, bool enableWildSearch, int minCharRequired, int autoSearchDelayInMilliSecond)
        {
            return string.Format(
                @"
                <script>
                $(function() {{
                    if (typeof dnn != 'undefined' && typeof dnn.searchSkinObject != 'undefined') {{
                        var searchSkinObject = new dnn.searchSkinObject({{
                            delayTriggerAutoSearch: {0},
                            minCharRequiredTriggerAutoSearch: {1},
                            searchType: '{2}',
                            enableWildSearch: {3},
                            cultureCode: '{4}',
                            portalId: {5}
                        }});
                        searchSkinObject.init();
                        
                        {6}
                    }}
                }});
                </script>",
                autoSearchDelayInMilliSecond,
                minCharRequired,
                "S",
                enableWildSearch.ToString().ToLowerInvariant(),
                System.Threading.Thread.CurrentThread.CurrentCulture.ToString(),
                PortalSettings.Current.PortalId,
                useDropDownList ? "if (typeof dnn.initDropdownSearch != 'undefined') { dnn.initDropdownSearch(searchSkinObject); }" : string.Empty);
        }

        private static string GetSeeMoreText()
        {
            return Localization.GetSafeJSString("SeeMoreResults", GetSkinsResourceFile(MyFileName));
        }

        private static string GetNoResultText()
        {
            return Localization.GetSafeJSString("NoResult", GetSkinsResourceFile(MyFileName));
        }

        private static string GetClearQueryText()
        {
            return Localization.GetSafeJSString("SearchClearQuery", GetSkinsResourceFile(MyFileName));
        }

        private static string GetPlaceholderText()
        {
            return Localization.GetSafeJSString("Placeholder", GetSkinsResourceFile(MyFileName));
        }
    }
}
