// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticToDynamicResourceOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.StaticToDynamicResource;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;

/// <summary>
/// Optimizer that converts <c>x:StaticResource</c> to <c>x:DynamicResource</c> if their key contain the DynamicMarker.
/// </summary>
public sealed class StaticToDynamicResourceOptimizer : IXamlOptimizer
{
    private const string StaticResourcePrefix = "{StaticResource ";

    private readonly StaticToDynamicResourceSettings staticToDynamicResourceSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticToDynamicResourceOptimizer"/> class with the specified settings.
    /// </summary>
    /// <param name="staticToDynamicResourceSettings">The static to dynamic resource settings.</param>
    public StaticToDynamicResourceOptimizer(StaticToDynamicResourceSettings staticToDynamicResourceSettings)
    {
        this.staticToDynamicResourceSettings = staticToDynamicResourceSettings;
    }

    /// <summary>
    /// Gets the supported platforms.
    /// </summary>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF];

    /// <summary>
    /// Optimizes the specified xaml document.
    /// </summary>
    /// <param name="xamlFiles">The xaml file.</param>
    /// <param name="xamlPlatformInfo">The xaml platform info.</param>
    /// <param name="projectInfo">The project info.</param>
    /// <returns>The optimization result.</returns>
    public async ValueTask<OptimizationResult> OptimizeAsync(IReadOnlyList<XamlFile> xamlFiles, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        var keyName = xamlPlatformInfo.XamlNamespace + "Key";
        var xamlFileChanges = new ConcurrentBag<XamlFileChange>();
        await xamlFiles.ParallelForEachAsync(
            new ParallelOptions { MaxDegreeOfParallelism = projectInfo.IsDebugging ? 1 : Environment.ProcessorCount },
            (xamlFile, token) =>
        {
            if (!xamlFile.Document.Root.HasValue())
            {
                return Task.CompletedTask;
            }

            var hasBeenOptimized = false;
            const string storyboard = "Storyboard";
            foreach (var xElement in this.DescendantsAndSelfWithFilter(xamlFile.Document.Root, xElement => xElement.Name.LocalName == storyboard))
            {
                foreach (var attribute in xElement.Attributes())
                {
                    const string dynamicResource = "{DynamicResource ";
                    if (attribute.Value.StartsWith(StaticResourcePrefix) &&
                        attribute.Value.AsSpan(StaticResourcePrefix.Length).Contains(
                            this.staticToDynamicResourceSettings.DynamicMarker.AsSpan(),
                            StringComparison.InvariantCulture) &&
                        !this.IsDefinedIsSameDocument(attribute.Value, xamlFile.Document.Root, keyName))
                    {
                        attribute.Value = dynamicResource +
                                          attribute.Value.AsSpan(StaticResourcePrefix.Length).ToString();
                        hasBeenOptimized = true;
                    }
                }
            }

            if (hasBeenOptimized)
            {
                xamlFileChanges.Add(XamlFileChange.Update(xamlFile));
            }

            return Task.CompletedTask;
        });

        return OptimizationResult.From(xamlFileChanges);
    }

    private IEnumerable<XElement> DescendantsAndSelfWithFilter(XElement rootXElement, Func<XElement, bool> skipElementPredicate)
    {
        foreach (var xElement in rootXElement.Elements())
        {
            if (skipElementPredicate(xElement))
            {
                continue;
            }

            yield return xElement;
            foreach (var childElement in this.DescendantsAndSelfWithFilter(xElement, skipElementPredicate))
            {
                yield return childElement;
            }
        }
    }

    private bool IsDefinedIsSameDocument(string attributeValue, XElement rootElement, XName keyName)
    {
        var resourceKey = attributeValue.AsSpan(StaticResourcePrefix.Length, attributeValue.Length - StaticResourcePrefix.Length - 1);
        foreach (var element in rootElement.DescendantsAndSelf())
        {
            var keyAttribute = element.Attribute(keyName);
            if (keyAttribute != null && keyAttribute.Value.AsSpan().Equals(resourceKey, StringComparison.InvariantCulture))
            {
                return true;
            }
        }

        return false;
    }
}
