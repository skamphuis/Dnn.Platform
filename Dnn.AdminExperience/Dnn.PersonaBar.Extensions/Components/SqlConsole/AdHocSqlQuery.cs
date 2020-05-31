﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using Newtonsoft.Json;

#endregion

namespace Dnn.PersonaBar.SqlConsole.Components
{
    [JsonObject]
    public class AdhocSqlQuery
    {
        [JsonProperty("connection")]
        public string ConnectionStringName { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("Timeout")]
        public int Timeout { get; set; }
    }
}
