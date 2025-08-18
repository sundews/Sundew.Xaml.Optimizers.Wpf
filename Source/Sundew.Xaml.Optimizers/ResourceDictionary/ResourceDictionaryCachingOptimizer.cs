// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryCachingOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

/// <summary>
/// Optimizes resource dictionaries using an <see cref="XDocument"/>.
/// </summary>
public class ResourceDictionaryCachingOptimizer : IXamlOptimizer
{
    private static readonly XNamespace SundewXamlOptimizationWpfNamespace = XNamespace.Get("clr-namespace:Sundew.Xaml.Optimizations;assembly=Sundew.Xaml.Wpf");
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly XName sundewXamlResourceDictionaryName;

    /// <summary>Initializes a new instance of the <see cref="ResourceDictionaryCachingOptimizer"/> class.</summary>
    /// <param name="xamlPlatformInfo">The framework XML definitions.</param>
    public ResourceDictionaryCachingOptimizer(XamlPlatformInfo xamlPlatformInfo)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.sundewXamlResourceDictionaryName = SundewXamlOptimizationWpfNamespace + Constants.ResourceDictionaryName;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms { get; } = new List<XamlPlatform> { XamlPlatform.WPF };

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
        foreach (var xElement in mergedResourceDictionaries.ToList())
        {
            var optimization = OptimizationProvider.GetOptimizationInfo(
                xElement,
                this.xamlPlatformInfo.SystemResourceDictionaryName);
            switch (optimization.OptimizationMode)
            {
                case OptimizationMode.Shared:
                    if (!hasSxoNamespace)
                    {
                        xDocument.Root.EnsureXmlNamespaceAttribute(
                            SundewXamlOptimizationWpfNamespace,
                            Constants.SxoPrefix,
                            this.xamlPlatformInfo.DefaultInsertAfterNamespaces);
                        hasSxoNamespace = true;
                    }

                    hasBeenOptimized = true;
                    xElement.ReplaceWith(new XElement(
                        this.sundewXamlResourceDictionaryName,
                        new XAttribute(Constants.SourceText, optimization.Source)));
                    break;
            }
        }

        return OptimizationResult.From(hasBeenOptimized, xDocument);
    }
}