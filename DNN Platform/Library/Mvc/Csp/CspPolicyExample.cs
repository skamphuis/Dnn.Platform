// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    using System;

    /// <summary>
    /// Example usage of the CSP contributors.
    /// </summary>
    public class CspPolicyExample
    {
        public static void Example()
        {
            // Create a Content Security Policy
            var csp = new ContentSecurityPolicy();

            // Add a source-based contributor for script sources
            var scriptSrcContributor = new SourceCspContributor(CspDirectiveType.ScriptSrc);
            scriptSrcContributor.AddSource(new CspSource(CspSourceType.Self));
            scriptSrcContributor.AddSource(new CspSource(CspSourceType.Host, "https://trusted-cdn.com"));
            csp.AddContributor(scriptSrcContributor);

            // Add a document-based contributor for sandbox
            var sandboxContributor = new DocumentCspContributor(CspDirectiveType.SandboxDirective, "allow-scripts allow-same-origin");
            csp.AddContributor(sandboxContributor);

            // Add a reporting contributor
            var reportingContributor = new ReportingCspContributor(CspDirectiveType.ReportUri);
            reportingContributor.AddReportingEndpoint("https://example.com/csp-report");
            csp.AddContributor(reportingContributor);

            csp.AddReportTo("https://example.com/csp-report");
            csp.AddReportUri("https://example.com/csp-report");

            // Generate the complete policy
            string policy = csp.GeneratePolicy();

            Console.WriteLine(policy);
        }
    }
}
