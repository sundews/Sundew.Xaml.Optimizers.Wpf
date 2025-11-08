// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FreezeResourceOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Freezing;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.Freezing.Internal;

/// <summary>Optimizes resources by adding freeze attribute.</summary>
public partial class FreezeResourceOptimizer : IXamlOptimizer
{
    private const string PoPrefix = "po";
    private const string True = "True";
    private const char SpaceCharacter = ' ';
    private const char CommaCharacter = ',';
    private static readonly XNamespace PresentationOptionsNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";
    private static readonly XName FreezeName = PresentationOptionsNamespace + "Freeze";
    private readonly string? unfreezeMarker;
    private readonly FreezeResourceSettings freezeResourceSettings;

    /// <summary>Initializes a new instance of the <see cref="FreezeResourceOptimizer"/> class.</summary>
    /// <param name="freezeResourceSettings">The freeze resource settings.</param>
    public FreezeResourceOptimizer(FreezeResourceSettings freezeResourceSettings)
    {
        this.freezeResourceSettings = freezeResourceSettings;

        this.unfreezeMarker = freezeResourceSettings.UnfreezeMarker;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms { get; } = [XamlPlatform.WPF];

    /// <inheritdoc/>
    public async ValueTask<OptimizationResult> OptimizeAsync(XamlFiles xamlFiles, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        var includedTypesBuilder = this.freezeResourceSettings.IncludeFrameworkTypes
            ? this.GetDefaultIncludedTypes(xamlPlatformInfo, DefaultFreezables)
            : ImmutableHashSet.CreateBuilder<XName>();
        var keyName = xamlPlatformInfo.XamlNamespace + "Key";

        foreach (var includedType in this.freezeResourceSettings.IncludedTypes)
        {
            var xName = XName.Get(includedType);
            if (xName.Namespace == XNamespace.None)
            {
                xName = xamlPlatformInfo.PresentationNamespace + includedType;
            }

            includedTypesBuilder.Add(xName);
        }

        foreach (var excludedType in this.freezeResourceSettings.ExcludedTypes)
        {
            var xName = XName.Get(excludedType);
            if (xName.Namespace == XNamespace.None)
            {
                xName = xamlPlatformInfo.PresentationNamespace + excludedType;
            }

            includedTypesBuilder.Remove(xName);
        }

        var includedTypes = includedTypesBuilder.ToImmutable();
        var xamlFileChanges = new ConcurrentBag<XamlFileChange>();
        await xamlFiles.ForEachAsync(
            (file, token) =>
            {
                var hasBeenOptimized = false;
                var rootElement = file.Document.Root;
                if (rootElement.HasValue)
                {
                    if (rootElement.Name == xamlPlatformInfo.SystemResourceDictionaryName)
                    {
                        this.TryOptimize(rootElement, rootElement, ref hasBeenOptimized, xamlPlatformInfo, keyName, includedTypes);

                        foreach (var resourcesElement in rootElement
                                     .Descendants()
                                     .Where(x => x.Name.LocalName.EndsWith(Constants.ResourcesName)))
                        {
                            this.TryOptimize(resourcesElement, rootElement, ref hasBeenOptimized, xamlPlatformInfo, keyName, includedTypes);
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

                        if (firstElement.Name == xamlPlatformInfo.SystemResourceDictionaryName)
                        {
                            elementToOptimize = firstElement;
                        }

                        this.TryOptimize(elementToOptimize, rootElement, ref hasBeenOptimized, xamlPlatformInfo, keyName, includedTypes);
                    }
                }

                if (hasBeenOptimized)
                {
                    xamlFileChanges.Add(XamlFileChange.Update(file));
                }

                return Task.CompletedTask;
            }).ConfigureAwait(false);

        return OptimizationResult.From(xamlFileChanges);
    }

    private void TryOptimize(XElement elementToOptimize, XElement rootElement, ref bool hasBeenOptimized, XamlPlatformInfo xamlPlatformInfo, XName keyName, ImmutableHashSet<XName> includedTypes)
    {
        var hasAddedNamespaces = false;
        foreach (var element in elementToOptimize.Elements()
                     .Where(x => includedTypes.Contains(x.Name)
                                 && this.AllowsFreezing(x, keyName)))
        {
            if (!hasAddedNamespaces)
            {
                var poAttribute = rootElement.EnsureXmlNamespaceAttribute(
                    PresentationOptionsNamespace,
                    PoPrefix,
                    xamlPlatformInfo.XamlNamespace,
                    xamlPlatformInfo.DesignerNamespace);

                rootElement.EnsureXmlNamespaceAttribute(
                    xamlPlatformInfo.MarkupCompatibilityNamespace,
                    xamlPlatformInfo.MarkupCompatibilityPrefix,
                    PresentationOptionsNamespace);

                var ignorableAttribute = rootElement.Attribute(xamlPlatformInfo.IgnorableName);
                if (ignorableAttribute != null)
                {
                    if (!ignorableAttribute.Value.Split(SpaceCharacter).Contains(poAttribute.Name.LocalName))
                    {
                        ignorableAttribute.Value += SpaceCharacter + poAttribute.Name.LocalName;
                    }
                    else if (!ignorableAttribute.Value.Split(CommaCharacter).Contains(poAttribute.Name.LocalName))
                    {
                        ignorableAttribute.Value += CommaCharacter + poAttribute.Name.LocalName;
                    }
                }
                else
                {
                    rootElement.Add(new XAttribute(xamlPlatformInfo.IgnorableName, poAttribute.Name.LocalName));
                }

                rootElement.EnsureXmlNamespaceAttribute(
                    PresentationOptionsNamespace,
                    poAttribute.Name.LocalName,
                    xamlPlatformInfo.XamlNamespace,
                    xamlPlatformInfo.DesignerNamespace);

                hasAddedNamespaces = true;
            }

            element.Add(new XAttribute(FreezeName, True));
            hasBeenOptimized = true;
        }
    }

    private bool AllowsFreezing(XElement xElement, XName keyName)
    {
        if (xElement.Attributes().FirstOrDefault(xAttribute => xAttribute.Name == FreezeName) != null)
        {
            return false;
        }

        if (this.unfreezeMarker == null)
        {
            return true;
        }

        return !xElement.Attribute(keyName)?.Value.Contains(this.unfreezeMarker) ?? false;
    }

    private System.Collections.Immutable.ImmutableHashSet<XName>.Builder GetDefaultIncludedTypes(XamlPlatformInfo xamlPlatformInfo, params string[] types)
    {
        var hashSet = ImmutableHashSet.CreateBuilder<XName>();
        foreach (var type in types)
        {
            hashSet.Add(xamlPlatformInfo.PresentationNamespace + type);
        }

        return hashSet;
    }
}