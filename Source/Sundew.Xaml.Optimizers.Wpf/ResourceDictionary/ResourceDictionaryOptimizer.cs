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
    private const string Source = "Source";
    private static readonly XamlType FallbackReplacementType = new XamlType(Constants.SxPrefix, Constants.SundewXamlOptimizationNamespace, Constants.ResourceDictionaryName);
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ResourceDictionarySettings resourceDictionarySettings;
    private readonly ProjectInfo projectInfo;
    private readonly XamlType defaultReplacementType;
    private readonly bool defaultReplaceUncategorized;

    /// <summary>Initializes a new instance of the <see cref="ResourceDictionaryOptimizer"/> class.</summary>
    /// <param name="xamlPlatformInfo">The framework XML definitions.</param>
    /// <param name="resourceDictionarySettings">The resource dictionary caching settings.</param>
    /// <param name="projectInfo">The project info.</param>
    public ResourceDictionaryOptimizer(ResourceDictionarySettings resourceDictionarySettings, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.resourceDictionarySettings = resourceDictionarySettings;
        this.projectInfo = projectInfo;
        this.defaultReplacementType = resourceDictionarySettings.DefaultReplacementType != null ? XamlType.TryParse(resourceDictionarySettings.DefaultReplacementType) ?? FallbackReplacementType : FallbackReplacementType;
        this.defaultReplaceUncategorized = this.resourceDictionarySettings.ReplaceUncategorized ?? this.xamlPlatformInfo.XamlPlatform == XamlPlatform.WPF;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF];

    /// <summary>Optimizes the xml document.</summary>
    /// <param name="xamlFiles">The xaml file.</param>
    /// <returns>A result with the optimized <see cref="XDocument"/>, if successful.</returns>
    public async ValueTask<OptimizationResult> OptimizeAsync(IReadOnlyList<XamlFile> xamlFiles)
    {
        var xamlFilesChanges = new ConcurrentBag<XamlFileChange>();
        await xamlFiles.ParallelForEachAsync(
            new ParallelOptions { MaxDegreeOfParallelism = this.projectInfo.IsDebugging ? 1 : Environment.ProcessorCount },
            (xamlFile, token) =>
        {
            var mergedResourceDictionaries = xamlFile.Document.XPathSelectElements(
                Constants.DefaultResourceDictionaryMergedDictionariesDefaultResourceDictionaryXPath,
                this.xamlPlatformInfo.XmlNamespaceResolver);
            var hasBeenOptimized = false;
            var hasSxoNamespace = false;
            if (!xamlFile.Document.Root.HasValue())
            {
                return Task.CompletedTask;
            }

            foreach (var xElement in mergedResourceDictionaries.ToList())
            {
                var optimization = OptimizationProvider.GetOptimizationInfo(
                    xElement,
                    this.defaultReplacementType,
                    this.defaultReplaceUncategorized,
                    this.resourceDictionarySettings.OptimizationMappings,
                    this.xamlPlatformInfo.SystemResourceDictionaryName);
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
                                this.xamlPlatformInfo.DefaultInsertAfterNamespaces);
                            hasSxoNamespace = true;
                        }

                        hasBeenOptimized = true;
                        xElement.ReplaceWith(new XElement(
                            optimization.ReplacementType.Namespace + optimization.ReplacementType.Name,
                            new XAttribute(Constants.SourceText, optimization.Source)));
                        break;
                }
            }

            if (hasBeenOptimized)
            {
                xamlFilesChanges.Add(XamlFileChange.Update(xamlFile));
            }

            return Task.CompletedTask;
        });

        return OptimizationResult.From(xamlFilesChanges);
    }
}