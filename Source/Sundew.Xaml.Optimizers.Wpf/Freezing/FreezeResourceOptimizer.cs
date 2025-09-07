// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FreezeResourceOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Freezing;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    private static readonly XNamespace PresentationOptionsNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";
    private static readonly XName FreezeName = PresentationOptionsNamespace + "Freeze";
    private readonly XName keyName;
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ProjectInfo projectInfo;
    private readonly HashSet<XName> includedTypes;
    private readonly string? unfreezeMarker;

    /// <summary>Initializes a new instance of the <see cref="FreezeResourceOptimizer"/> class.</summary>
    /// <param name="xamlPlatformInfo">The xaml platform information.</param>
    /// <param name="freezeResourceSettings">The freeze resource settings.</param>
    /// <param name="projectInfo">The project info.</param>
    public FreezeResourceOptimizer(FreezeResourceSettings freezeResourceSettings, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.projectInfo = projectInfo;
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

        this.keyName = this.xamlPlatformInfo.XamlNamespace + "Key";
        this.unfreezeMarker = freezeResourceSettings.UnfreezeMarker;
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms { get; } = [XamlPlatform.WPF];

    /// <inheritdoc/>
    public async ValueTask<OptimizationResult> OptimizeAsync(IReadOnlyList<XamlFile> xamlFiles)
    {
        var xamlFileChanges = new ConcurrentBag<XamlFileChange>();
        await xamlFiles.ParallelForEachAsync(
            new ParallelOptions { MaxDegreeOfParallelism = this.projectInfo.IsDebugging ? 1 : Environment.ProcessorCount },
            (file, token) =>
            {
                var hasBeenOptimized = false;
                var rootElement = file.Document.Root;
                if (rootElement.HasValue())
                {
                    if (rootElement.Name == this.xamlPlatformInfo.SystemResourceDictionaryName)
                    {
                        this.TryOptimize(rootElement, rootElement, ref hasBeenOptimized);

                        foreach (var resourcesElement in rootElement
                                     .Descendants()
                                     .Where(x => x.Name.LocalName.EndsWith(Constants.ResourcesName)))
                        {
                            this.TryOptimize(resourcesElement, rootElement, ref hasBeenOptimized);
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

                        this.TryOptimize(elementToOptimize, rootElement, ref hasBeenOptimized);
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

    private void TryOptimize(XElement elementToOptimize, XElement rootElement, ref bool hasBeenOptimized)
    {
        var hasAddedNamespaces = false;
        foreach (var element in elementToOptimize.Elements()
                     .Where(x => this.includedTypes.Contains(x.Name)
                                 && this.AllowsFreezing(x)))
        {
            if (!hasAddedNamespaces)
            {
                var poAttribute = rootElement.EnsureXmlNamespaceAttribute(
                    PresentationOptionsNamespace,
                    PoPrefix,
                    this.xamlPlatformInfo.XamlNamespace,
                    this.xamlPlatformInfo.DesignerNamespace);

                rootElement.EnsureXmlNamespaceAttribute(
                    this.xamlPlatformInfo.MarkupCompatibilityNamespace,
                    this.xamlPlatformInfo.MarkupCompatibilityPrefix,
                    PresentationOptionsNamespace);

                var ignorableAttribute = rootElement.Attribute(this.xamlPlatformInfo.IgnorableName);
                if (ignorableAttribute != null)
                {
                    if (!ignorableAttribute.Value.Split(SpaceCharacter).Contains(PoPrefix))
                    {
                        ignorableAttribute.Value += SpaceCharacter + poAttribute.Name.LocalName;
                    }
                }
                else
                {
                    rootElement.Add(new XAttribute(this.xamlPlatformInfo.IgnorableName, poAttribute.Name.LocalName));
                }

                hasAddedNamespaces = true;
            }

            element.Add(new XAttribute(FreezeName, True));
            hasBeenOptimized = true;
        }
    }

    private bool AllowsFreezing(XElement xElement)
    {
        if (xElement.Attributes().FirstOrDefault(xAttribute => xAttribute.Name == FreezeName) != null)
        {
            return false;
        }

        if (this.unfreezeMarker == null)
        {
            return true;
        }

        return !xElement.Attribute(this.keyName)?.Value.Contains(this.unfreezeMarker) ?? false;
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