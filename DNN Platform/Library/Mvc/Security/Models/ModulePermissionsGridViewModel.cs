// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Security.Permissions.Models
{
    using System.Collections.Generic;

    using DotNetNuke.Security.Permissions.Controllers;

    public class ModulePermissionsGridViewModel : PermissionsGridViewModel
    {
        public int ModuleId { get; set; }

        public int TabId { get; set; }

        public bool InheritViewPermissionsFromTab { get; set; }

        public List<RoleModel> RolePermissions { get; set; }
    }
}
