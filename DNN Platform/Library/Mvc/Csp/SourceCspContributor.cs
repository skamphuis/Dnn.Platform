// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Contributor for fetch directives (sources-based directives).
    /// </summary>
    public class SourceCspContributor : BaseCspContributor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCspContributor"/> class.
        /// </summary>
        /// <param name="directiveType">The directive type to create the contributor for.</param>
        public SourceCspContributor(CspDirectiveType directiveType)
        {
            this.DirectiveType = directiveType;
        }

        /// <summary>
        /// Gets collection of allowed sources.
        /// </summary>
        private List<CspSource> Sources { get; } = new List<CspSource>();

        /// <summary>
        /// Adds a source to the contributor.
        /// </summary>
        /// <param name="source">The source to add.</param>
        public void AddSource(CspSource source)
        {
            if (!this.Sources.Any(s => s.Type == source.Type && s.Value == source.Value))
            {
                this.Sources.Add(source);
            }
        }

        /// <summary>
        /// Removes a source from the contributor.
        /// </summary>
        /// <param name="sourceType">The type of the source to remove.</param>
        public void RemoveSources(CspSourceType sourceType)
        {
            this.Sources.RemoveAll(s => s.Type == sourceType);
        }

        /// <summary>
        /// Generates the directive string.
        /// </summary>
        /// <returns>The directive string.</returns>
        public override string GenerateDirective()
        {
            if (!this.Sources.Any())
            {
                return string.Empty;
            }

            return $"{CspDirectiveNameMapper.GetDirectiveName(this.DirectiveType)} {string.Join(" ", this.Sources.Select(s => s.ToString()))}";
        }

        /// <summary>
        /// Gets sources by type.
        /// </summary>
        /// <param name="type">The type of sources to get.</param>
        /// <returns>The sources of the specified type.</returns>
        public IEnumerable<CspSource> GetSourcesByType(CspSourceType type)
        {
            return this.Sources.Where(s => s.Type == type);
        }
    }
}
