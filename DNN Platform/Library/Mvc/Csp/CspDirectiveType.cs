// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    /// <summary>
    /// Represents different types of Content Security Policy directives.
    /// </summary>
    public enum CspDirectiveType
    {
        // Fetch directives
        DefaultSrc,
        ScriptSrc,
        StyleSrc,
        ImgSrc,
        ConnectSrc,
        FontSrc,
        ObjectSrc,
        MediaSrc,
        FrameSrc,

        // Document directives
        BaseUri,
        PluginTypes,
        SandboxDirective,

        // Navigation directives
        FormAction,
        FrameAncestors,

        // Reporting directives
        ReportUri,
        ReportTo,
    }
}
