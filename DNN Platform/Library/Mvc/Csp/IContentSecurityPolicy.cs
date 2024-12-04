// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

namespace DotNetNuke.Web.Mvc.Csp
{
    /// <summary>
    /// Interface définissant les opérations de gestion de la Content Security Policy.
    /// </summary>
    public interface IContentSecurityPolicy
    {
        string Nonce { get; }

        /// <summary>
        /// Ajoute un contributeur à la politique.
        /// </summary>
        void AddContributor(BaseCspContributor contributor);

        /// <summary>
        /// Ajoute une source par défaut à la politique.
        /// </summary>
        void AddDefaultSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source de script à la politique.
        /// </summary>
        void AddScriptSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Supprimer une source de script à la politique.
        /// </summary>
        void RemoveScriptSources(CspSourceType cspSourceType);

        /// <summary>
        /// Ajoute une source de style à la politique.
        /// </summary>
        void AddStyleSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source d'image à la politique.
        /// </summary>
        void AddImgSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source de connexion à la politique.
        /// </summary>
        void AddConnectSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source de police à la politique.
        /// </summary>
        void AddFontSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source d'objet à la politique.
        /// </summary>
        void AddObjectSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source média à la politique.
        /// </summary>
        void AddMediaSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une source de frame à la politique.
        /// </summary>
        void AddFrameSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute une base URI à la politique.
        /// </summary>
        void AddBaseUriSource(CspSourceType cspSourceType, string value = null);

        /// <summary>
        /// Ajoute des types de plugins à la politique.
        /// </summary>
        void AddPluginTypes(string value);

        /// <summary>
        /// Ajoute une directive sandbox à la politique.
        /// </summary>
        void AddSandboxDirective(string value);

        /// <summary>
        /// Ajoute une action de formulaire à la politique.
        /// </summary>
        void AddFormAction(CspSourceType sourceType, string value);

        /// <summary>
        /// Ajoute des ancêtres de frame à la politique.
        /// </summary>
        void AddFrameAncestors(CspSourceType sourceType, string value);

        /// <summary>
        /// Ajoute une URI de rapport à la politique.
        /// </summary>
        void AddReportUri(string value);

        /// <summary>
        /// Ajoute une destination de rapport à la politique.
        /// </summary>
        void AddReportTo(string value);

        /// <summary>
        /// Génère la politique de sécurité complète.
        /// </summary>
        /// <returns>La politique de sécurité complète sous forme de chaîne.</returns>
        string GeneratePolicy();
    }
}
