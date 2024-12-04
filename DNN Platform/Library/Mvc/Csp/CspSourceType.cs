// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    /// <summary>
    /// Represents different types of Content Security Policy source types.
    /// </summary>
    public enum CspSourceType
    {
        Host,           // Specific domains
        Scheme,         // Protocols like https:, data:
        Self,           // 'self'
        Inline,         // 'unsafe-inline'
        Eval,           // 'unsafe-eval'
        Nonce,          // Cryptographic nonce
        Hash,            // Cryptographic hash
        None,
        StrictDynamic,
    }
}
