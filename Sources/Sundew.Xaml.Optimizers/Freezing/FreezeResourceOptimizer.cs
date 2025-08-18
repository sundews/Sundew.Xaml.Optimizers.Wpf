// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FreezeResourceOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Freezing;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Freezing.Internal;

/// <summary>Optimizes resources by adding freeze attribute.</summary>
public partial class FreezeResourceOptimizer : IXamlOptimizer
{
    private const string PoPrefix = "po";
    private const string True = "True";
    private const char SpaceCharacter = ' ';
    private static readonly XNamespace PresentationOptionsNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";
    private static readonly XName FreezeName = PresentationOptionsNamespace + "Freeze";
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly HashSet<XName> includedTypes;

    /// <summary>Initializes a new instance of the <see cref="FreezeResourceOptimizer"/> class.</summary>
    /// <param name="xamlPlatformInfo">The xaml platform information.</param>
    /// <param name="freezeResourceSettings">The freeze resource settings.</param>
    public FreezeResourceOptimizer(XamlPlatformInfo xamlPlatformInfo, FreezeResourceSettings freezeResourceSettings)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.includedTypes = freezeResourceSettings.IncludeFrameworkTypes
            ? this.GetDefaultIncludedTypes(DefaultFreezables)
            : new HashSet<XName>();
        foreach (var includedType in freezeResourceSettings.IncludedTypes)
        {
            var xName = XName.Get(includedType);
            if (xName.Namespace == XNamespace.None)
            {
                xName = this.xamlPlatformInfo.PresentationNamespace + includedType;
            }

            this.includedTypes.Add(xName);
        }

        foreach (var excludedType in freezeResourceSettings.ExcludedTypes)
        {
            var xName = XName.Get(excludedType);
            if (xName.Namespace == XNamespace.None)
            {
                xName = this.xamlPlatformInfo.PresentationNamespace + excludedType;
            }

            this.includedTypes.Remove(xName);
        }
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms { get; } = new List<XamlPlatform> { XamlPlatform.WPF };

    /// <summary>Optimizes the specified xml document.</summary>
    /// <param name="xDocument">The xml document.</param>
    /// <param name="xamlFile">The xaml file reference.</param>
    /// <returns>The result of the xaml optimization.</returns>
    public OptimizationResult Optimize(XDocument xDocument, IFileReference xamlFile)
    {
        var hasBeenOptimized = false;
        var rootElement = xDocument.Root;
        if (rootElement != null)
        {
            if (rootElement.Name == this.xamlPlatformInfo.SystemResourceDictionaryName)
            {
                this.TryOptimize(rootElement, xDocument, ref hasBeenOptimized);

                foreach (var resourcesElement in rootElement
                             .Descendants()
                             .Where(x => x.Name.LocalName.EndsWith(Constants.ResourcesName)))
                {
                    this.TryOptimize(resourcesElement, xDocument, ref hasBeenOptimized);
                }
            }

            foreach (var resourcesElement in rootElement
                         .Descendants()
                         .Where(x => x.Name.LocalName.EndsWith(Constants.ResourcesName)))
            {
                var elementToOptimize = resourcesElement;
                var firstElement = resourcesElement.Elements().FirstOrDefault();
                if (firstElement == null)
                {
                    break;
                }

                if (firstElement.Name == this.xamlPlatformInfo.SystemResourceDictionaryName)
                {
                    elementToOptimize = firstElement;
                }

                this.TryOptimize(elementToOptimize, xDocument, ref hasBeenOptimized);
            }
        }

        return OptimizationResult.From(hasBeenOptimized, xDocument);
    }

    private void TryOptimize(XElement elementToOptimize, XDocument xDocument, ref bool hasBeenOptimized)
    {
        var hasAddedNamespaces = false;
        foreach (var element in elementToOptimize.Elements()
                     .Where(x => this.includedTypes.Contains(x.Name)
                                 && x.Attributes().FirstOrDefault(xAttribute => xAttribute.Name == FreezeName) == null))
        {
            if (!hasAddedNamespaces)
            {
                var poAttribute = xDocument.Root.EnsureXmlNamespaceAttribute(
                    PresentationOptionsNamespace,
                    PoPrefix,
                    this.xamlPlatformInfo.XamlNamespace,
                    this.xamlPlatformInfo.DesignerNamespace);

                xDocument.Root.EnsureXmlNamespaceAttribute(
                    this.xamlPlatformInfo.MarkupCompatibilityNamespace,
                    this.xamlPlatformInfo.MarkupCompatibilityPrefix,
                    PresentationOptionsNamespace);

                var ignorableAttribute = xDocument.Root.Attribute(this.xamlPlatformInfo.IgnorableName);
                if (ignorableAttribute != null)
                {
                    if (!ignorableAttribute.Value.Split(SpaceCharacter).Contains(PoPrefix))
                    {
                        ignorableAttribute.Value += SpaceCharacter + poAttribute.Name.LocalName;
                    }
                }
                else
                {
                    xDocument.Root.Add(new XAttribute(this.xamlPlatformInfo.IgnorableName, poAttribute.Name.LocalName));
                }

                hasAddedNamespaces = true;
            }

            element.Add(new XAttribute(FreezeName, True));
            hasBeenOptimized = true;
        }
    }

    private HashSet<XName> GetDefaultIncludedTypes(params string[] types)
    {
        var hashSet = new HashSet<XName>();
        foreach (var type in types)
        {
            hashSet.Add(this.xamlPlatformInfo.PresentationNamespace + type);
        }

        return hashSet;
    }
}