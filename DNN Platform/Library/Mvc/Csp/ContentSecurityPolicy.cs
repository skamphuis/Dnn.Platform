// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages the entire Content Security Policy.
    /// </summary>
    public class ContentSecurityPolicy : IContentSecurityPolicy
    {
        private string nonce;

        /// <summary>Initializes a new instance of the <see cref="ContentSecurityPolicy"/> class.</summary>
        public ContentSecurityPolicy()
        {
        }

        public string Nonce
        {
            get
            {
                if (this.nonce == null)
                {
                    var nonceBytes = new byte[32];
                    var generator = System.Security.Cryptography.RandomNumberGenerator.Create();
                    generator.GetBytes(nonceBytes);
                    this.nonce = System.Convert.ToBase64String(nonceBytes);
                }

                return this.nonce;
            }
        }

        /// <summary>
        /// Gets collection of CSP contributors.
        /// </summary>
        private List<BaseCspContributor> Contributors { get; } = new List<BaseCspContributor>();

        /// <summary>
        /// Adds a contributor to the policy.
        /// </summary>
        public void AddContributor(BaseCspContributor contributor)
        {
            // Remove any existing contributor of the same directive type
            this.Contributors.RemoveAll(c => c.DirectiveType == contributor.DirectiveType);
            this.Contributors.Add(contributor);
        }

        public void AddDefaultSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.DefaultSrc, cspSourceType, value);
        }

        public void AddScriptSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.ScriptSrc, cspSourceType, value);
        }

        public void RemoveScriptSources(CspSourceType cspSourceType)
        {
            this.RemoveSources(CspDirectiveType.ScriptSrc, cspSourceType);
        }

        public void AddStyleSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.StyleSrc, cspSourceType, value);
        }

        public void AddImgSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.ImgSrc, cspSourceType, value);
        }

        public void AddConnectSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.ConnectSrc, cspSourceType, value);
        }

        public void AddFontSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.FontSrc, cspSourceType, value);
        }

        public void AddObjectSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.ObjectSrc, cspSourceType, value);
        }

        public void AddMediaSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.MediaSrc, cspSourceType, value);
        }

        public void AddFrameSource(CspSourceType cspSourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.FrameSrc, cspSourceType, value);
        }

        public void AddBaseUriSource(CspSourceType sourceType, string value = null)
        {
            this.AddSource(CspDirectiveType.BaseUri, sourceType, value);
        }

        public void AddPluginTypes(string value)
        {
            this.AddDocumentDirective(CspDirectiveType.PluginTypes, value);
        }

        public void AddSandboxDirective(string value)
        {
            this.AddDocumentDirective(CspDirectiveType.SandboxDirective, value);
        }

        public void AddFormAction(CspSourceType sourceType, string value)
        {
            this.AddNavigationDirective(CspDirectiveType.FormAction, sourceType, value);
        }

        public void AddFrameAncestors(CspSourceType sourceType, string value)
        {
            this.AddNavigationDirective(CspDirectiveType.FrameAncestors, sourceType, value);
        }

        public void AddReportUri(string value)
        {
            this.AddReportingDirective(CspDirectiveType.ReportUri, value);
        }

        public void AddReportTo(string value)
        {
            this.AddReportingDirective(CspDirectiveType.ReportTo, value);
        }

        /// <summary>
        /// Generates the complete Content Security Policy.
        /// </summary>
        /// <returns>The complete Content Security Policy.</returns>
        public string GeneratePolicy()
        {
            return string.Join(
                "; ",
                this.Contributors
                    .Select(c => c.GenerateDirective())
                    .Where(d => !string.IsNullOrEmpty(d)));
        }

        private void AddSource(CspDirectiveType directiveType, CspSourceType sourceType, string value = null)
        {
            var contributor = this.Contributors.FirstOrDefault(c => c.DirectiveType == directiveType) as SourceCspContributor;
            if (contributor == null)
            {
                contributor = new SourceCspContributor(directiveType);
                this.AddContributor(contributor);
            }

            if (sourceType == CspSourceType.Nonce && string.IsNullOrEmpty(value))
            {
                value = this.Nonce;
            }

            contributor.AddSource(new CspSource(sourceType, value));
        }

        private void RemoveSources(CspDirectiveType directiveType, CspSourceType sourceType)
        {
            var contributor = this.Contributors.FirstOrDefault(c => c.DirectiveType == directiveType) as SourceCspContributor;
            if (contributor == null)
            {
                contributor = new SourceCspContributor(directiveType);
                this.AddContributor(contributor);
            }

            contributor.RemoveSources(sourceType);
        }

        private void AddDocumentDirective(CspDirectiveType directiveType, string value)
        {
            var contributor = this.Contributors.FirstOrDefault(c => c.DirectiveType == directiveType) as DocumentCspContributor;
            if (contributor == null)
            {
                contributor = new DocumentCspContributor(directiveType, value);
                this.AddContributor(contributor);
            }

            contributor.SetDirectiveValue(value);
        }

        private void AddNavigationDirective(CspDirectiveType directiveType, CspSourceType sourceType, string value = null)
        {
            var contributor = this.Contributors.FirstOrDefault(c => c.DirectiveType == directiveType) as SourceCspContributor;
            if (contributor == null)
            {
                contributor = new SourceCspContributor(directiveType);
                this.AddContributor(contributor);
            }

            contributor.AddSource(new CspSource(sourceType, value));
        }

        private void AddReportingDirective(CspDirectiveType directiveType, string value)
        {
            var contributor = this.Contributors.FirstOrDefault(c => c.DirectiveType == directiveType) as ReportingCspContributor;
            if (contributor == null)
            {
                contributor = new ReportingCspContributor(directiveType);
                this.AddContributor(contributor);
            }

            contributor.AddReportingEndpoint(value);
        }
    }
}
