// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

/// <summary>
/// Optimizes resource dictionaries using an <see cref="XDocument"/>.
/// </summary>
public class ResourceDictionaryOptimizer : IXamlOptimizer
{
    private static readonly XamlType FallbackReplacementType = new XamlType(Constants.SxPrefix, Constants.SundewXamlOptimizationWpfNamespace, Constants.ResourceDictionaryName);
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ResourceDictionarySettings resourceDictionarySettings;
    private readonly XamlType defaultReplacementType;
    private readonly bool defaultReplaceUncategorized;

    /// <summary>Initializes a new instance of the <see cref="ResourceDictionaryOptimizer"/> class.</summary>
    /// <param name="xamlPlatformInfo">The framework XML definitions.</param>
    /// <param name="resourceDictionarySettings">The resource dictionary caching settings.</param>
    public ResourceDictionaryOptimizer(XamlPlatformInfo xamlPlatformInfo, ResourceDictionarySettings resourceDictionarySettings)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.resourceDictionarySettings = resourceDictionarySettings;
        this.defaultReplacementType = resourceDictionarySettings.DefaultReplacementType != null ? XamlType.TryParse(resourceDictionarySettings.DefaultReplacementType) ?? FallbackReplacementType : FallbackReplacementType;
        this.defaultReplaceUncategorized = this.resourceDictionarySettings.ReplaceUncategorized ?? this.xamlPlatformInfo.XamlPlatform == XamlPlatform.WPF;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF, XamlPlatform.WinUI, XamlPlatform.Avalonia, XamlPlatform.Maui, XamlPlatform.UWP, XamlPlatform.XF];

    /// <summary>Optimizes the xml document.</summary>
    /// <param name="xDocument">The xml document.</param>
    /// <param name="xamlFile">The xaml file info.</param>
    /// <returns>A result with the optimized <see cref="XDocument"/>, if successful.</returns>
    public OptimizationResult Optimize(XDocument xDocument, IFileReference xamlFile)
    {
        var mergedResourceDictionaries = xDocument.XPathSelectElements(
            Constants.DefaultResourceDictionaryMergedDictionariesDefaultResourceDictionaryXPath,
            this.xamlPlatformInfo.XmlNamespaceResolver);
        var hasBeenOptimized = false;
        var hasSxoNamespace = false;
        if (!xDocument.Root.HasValue())
        {
            return OptimizationResult.None();
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
                    xElement.ReplaceWith(new XComment(@$"<{xElement.Name.LocalName} Source=""{optimization.Source}""/> was commented out by ResourceDictionaryOptimizer"));
                    hasBeenOptimized = true;
                    break;
                case OptimizationAction.Replace:
                    if (!hasSxoNamespace)
                    {
                        xDocument.Root.EnsureXmlNamespaceAttribute(
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

        return OptimizationResult.From(hasBeenOptimized, xDocument);
    }
}