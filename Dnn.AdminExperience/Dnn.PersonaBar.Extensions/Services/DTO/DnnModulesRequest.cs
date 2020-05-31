﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;

namespace Dnn.PersonaBar.Pages.Services.Dto
{
    public class DnnModulesRequest
    {
        public Guid UniqueId { get; set; }
        public List<DnnModuleDto> Modules { get; set; }
    }
}
