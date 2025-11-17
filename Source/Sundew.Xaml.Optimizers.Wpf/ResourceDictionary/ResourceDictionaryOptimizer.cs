// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.ResourceDictionary.Internal;

/// <summary>
/// Optimizes resource dictionaries using an <see cref="XDocument"/>.
/// </summary>
public class ResourceDictionaryOptimizer : IXamlOptimizer
{
    internal const string CategoryNotMapped = "RDO0001";
    private const string Source = "Source";
    private static readonly XamlType FallbackReplacementType = new XamlType(Constants.SxPrefix, Constants.SundewXamlOptimizationNamespace, Constants.ResourceDictionaryName);
    private readonly ResourceDictionarySettings resourceDictionarySettings;

    /// <summary>Initializes a new instance of the <see cref="ResourceDictionaryOptimizer"/> class.</summary>
    /// <param name="resourceDictionarySettings">The resource dictionary caching settings.</param>
    public ResourceDictionaryOptimizer(ResourceDictionarySettings resourceDictionarySettings)
    {
        this.resourceDictionarySettings = resourceDictionarySettings;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF];

    /// <summary>Optimizes the xml document.</summary>
    /// <param name="xamlFiles">The xaml file.</param>
    /// <param name="xamlPlatformInfo">The xaml platform info.</param>
    /// <param name="projectInfo">The project info.</param>
    /// <returns>A result with the optimized <see cref="XDocument"/>, if successful.</returns>
    public async ValueTask<OptimizationResult> OptimizeAsync(XamlFiles xamlFiles, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        var defaultReplacementType = this.resourceDictionarySettings.DefaultReplacementType != null ? XamlType.TryParse(this.resourceDictionarySettings.DefaultReplacementType) ?? FallbackReplacementType : FallbackReplacementType;
        var defaultReplaceUncategorized = this.resourceDictionarySettings.ReplaceUncategorized ?? xamlPlatformInfo.XamlPlatform == XamlPlatform.WPF;

        var xamlFilesChanges = new ConcurrentBag<XamlFileChange>();
        var xamlDiagnostics = new ConcurrentBag<XamlDiagnostic>();
        await xamlFiles.ForEachAsync(
            (xamlFile, token) =>
        {
            if (!xamlFile.Document.Root.HasValue)
            {
                return Task.CompletedTask;
            }

            var mergedResourceDictionaries = xamlFile.Document.XPathSelectElements(
                Constants.DefaultResourceDictionaryMergedDictionariesDefaultResourceDictionaryXPath,
                xamlPlatformInfo.XmlNamespaceResolver);

            var hasBeenOptimized = false;
            var hasSxoNamespace = false;
            foreach (var xElement in mergedResourceDictionaries.ToList())
            {
                var optimization = OptimizationProvider.GetOptimizationInfo(
                    xElement,
                    defaultReplacementType,
                    defaultReplaceUncategorized,
                    this.resourceDictionarySettings.OptimizationMappings,
                    xamlPlatformInfo.SystemResourceDictionaryName,
                    xamlFile.Reference);
                switch (optimization.OptimizationAction)
                {
                    case OptimizationAction.None:
                        break;
                    case OptimizationAction.Remove:
                        xElement.Attribute(Source)?.Remove();
                        hasBeenOptimized = true;
                        break;
                    case OptimizationAction.Replace:
                        if (!hasSxoNamespace)
                        {
                            xamlFile.Document.Root.EnsureXmlNamespaceAttribute(
                                optimization.ReplacementType.Namespace,
                                optimization.ReplacementType.Prefix,
                                xamlPlatformInfo.DefaultInsertAfterNamespaces);
                            hasSxoNamespace = true;
                        }

                        hasBeenOptimized = true;
                        xElement.ReplaceWith(new XElement(
                            optimization.ReplacementType.Namespace + optimization.ReplacementType.Name,
                            new XAttribute(Constants.SourceText, optimization.Source)));
                        break;
                }

                if (optimization.XamlDiagnostic.HasValue)
                {
                    xamlDiagnostics.Add(optimization.XamlDiagnostic);
                }
            }

            if (hasBeenOptimized)
            {
                xamlFilesChanges.Add(XamlFileChange.Update(xamlFile));
            }

            return Task.CompletedTask;
        });

        return OptimizationResult.From(xamlFilesChanges, xamlDiagnostics: xamlDiagnostics);
    }
}