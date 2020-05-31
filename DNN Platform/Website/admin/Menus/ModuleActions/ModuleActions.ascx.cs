﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;
using DotNetNuke.Collections;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Containers;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;

// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable InconsistentNaming

// ReSharper disable CheckNamespace
namespace DotNetNuke.Admin.Containers
// ReSharper restore CheckNamespace
{
    public partial class ModuleActions : ActionBase
    {
        private readonly List<int> validIDs = new List<int>();
        
        protected string AdminActionsJSON { get; set; }

        protected string AdminText
        {
            get { return Localization.GetString("ModuleGenericActions.Action", Localization.GlobalResourceFile); }
        }

        protected string CustomActionsJSON { get; set; }

        protected string CustomText
        {
            get { return Localization.GetString("ModuleSpecificActions.Action", Localization.GlobalResourceFile); }
        }

        protected bool DisplayQuickSettings { get; set; }

        protected string MoveText
        {
            get { return Localization.GetString(ModuleActionType.MoveRoot, Localization.GlobalResourceFile); }
        }

        protected string Panes { get; set; }

        protected bool SupportsMove { get; set; }

        protected bool SupportsQuickSettings { get; set; }

        protected bool IsShared { get; set; }

        protected string LocalizeString(string key)
        {
            return Localization.GetString(key, Localization.GlobalResourceFile);
        }

        protected string ModuleTitle { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ID = "ModuleActions";

            actionButton.Click += actionButton_Click;

            JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            ClientResourceManager.RegisterStyleSheet(Page, "~/admin/menus/ModuleActions/ModuleActions.css", FileOrder.Css.ModuleCss);
            ClientResourceManager.RegisterStyleSheet(Page, "~/Resources/Shared/stylesheets/dnnicons/css/dnnicon.min.css", FileOrder.Css.ModuleCss);
            ClientResourceManager.RegisterScript(Page, "~/admin/menus/ModuleActions/ModuleActions.js");

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }

        void actionButton_Click(object sender, EventArgs e)
        {
            ProcessAction(Request.Params["__EVENTARGUMENT"]);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AdminActionsJSON = "[]";
            CustomActionsJSON = "[]";
            Panes = "[]";
            try
            {
                SupportsQuickSettings = false;
                DisplayQuickSettings = false;
                ModuleTitle = ModuleContext.Configuration.ModuleTitle;
                var moduleDefinitionId = ModuleContext.Configuration.ModuleDefID;
                var quickSettingsControl = ModuleControlController.GetModuleControlByControlKey("QuickSettings", moduleDefinitionId);

                if (quickSettingsControl != null)
                {
                    SupportsQuickSettings = true;
                    var control  = ModuleControlFactory.LoadModuleControl(Page, ModuleContext.Configuration, "QuickSettings", quickSettingsControl.ControlSrc);
                    control.ID += ModuleContext.ModuleId;
                    quickSettings.Controls.Add(control);

                    DisplayQuickSettings = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("QS_FirstLoad", true);
                    ModuleController.Instance.UpdateModuleSetting(ModuleContext.ModuleId, "QS_FirstLoad", "False");

                    ClientResourceManager.RegisterScript(Page, "~/admin/menus/ModuleActions/dnnQuickSettings.js");
                }

                if (ActionRoot.Visible)
                {
                    //Add Menu Items
                    foreach (ModuleAction rootAction in ActionRoot.Actions)
                    {
                        //Process Children
                        var actions = new List<ModuleAction>();
                        foreach (ModuleAction action in rootAction.Actions)
                        {
                            if (action.Visible)
                            {
                                if ((EditMode && Globals.IsAdminControl() == false) ||
                                    (action.Secure != SecurityAccessLevel.Anonymous && action.Secure != SecurityAccessLevel.View))
                                {
                                    if (!action.Icon.Contains("://")
                                            && !action.Icon.StartsWith("/")
                                            && !action.Icon.StartsWith("~/"))
                                    {
                                        action.Icon = "~/images/" + action.Icon;
                                    }
                                    if (action.Icon.StartsWith("~/"))
                                    {
                                        action.Icon = Globals.ResolveUrl(action.Icon);
                                    }

                                    actions.Add(action);

                                    if(String.IsNullOrEmpty(action.Url))
                                    {
                                        validIDs.Add(action.ID);
                                    }
                                }
                            }

                        }

                        var oSerializer = new JavaScriptSerializer();
                        if (rootAction.Title == Localization.GetString("ModuleGenericActions.Action", Localization.GlobalResourceFile))
                        {
                            AdminActionsJSON = oSerializer.Serialize(actions);
                        }
                        else
                        {
                            if (rootAction.Title == Localization.GetString("ModuleSpecificActions.Action", Localization.GlobalResourceFile))
                            {
                                CustomActionsJSON = oSerializer.Serialize(actions);
                            }
                            else
                            {
                                SupportsMove = (actions.Count > 0);
                                Panes = oSerializer.Serialize(PortalSettings.ActiveTab.Panes);
                            }
                        }
                    }
                    IsShared = ModuleContext.Configuration.AllTabs
                        || PortalGroupController.Instance.IsModuleShared(ModuleContext.ModuleId, PortalController.Instance.GetPortal(PortalSettings.PortalId))
                        || TabController.Instance.GetTabsByModuleID(ModuleContext.ModuleId).Count > 1;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            foreach(int id in validIDs)
            {
                Page.ClientScript.RegisterForEventValidation(actionButton.UniqueID, id.ToString());
            }
        }
    }    
}
