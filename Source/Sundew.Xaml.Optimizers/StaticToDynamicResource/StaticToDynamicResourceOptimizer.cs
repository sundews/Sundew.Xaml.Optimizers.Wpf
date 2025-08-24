// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticToDynamicResourceOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.StaticToDynamicResource;

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;

/// <summary>
/// Optimizer that converts <c>x:StaticResource</c> to <c>x:DynamicResource</c> if their key contain the DynamicMarker.
/// </summary>
public sealed class StaticToDynamicResourceOptimizer : IXamlOptimizer
{
    private const string StaticResourcePrefix = "{StaticResource ";

    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly StaticToDynamicResourceSettings staticToDynamicResourceSettings;
    private readonly XName keyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaticToDynamicResourceOptimizer"/> class with the specified settings.
    /// </summary>
    /// <param name="xamlPlatformInfo">The xaml platformIinfo.</param>
    /// <param name="staticToDynamicResourceSettings">The static to dynamic resource settings.</param>
    public StaticToDynamicResourceOptimizer(XamlPlatformInfo xamlPlatformInfo, StaticToDynamicResourceSettings staticToDynamicResourceSettings)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.staticToDynamicResourceSettings = staticToDynamicResourceSettings;
        this.keyName = xamlPlatformInfo.XamlNamespace + "Key";
    }

    /// <summary>
    /// Gets the supported platforms.
    /// </summary>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF, XamlPlatform.WinUI, XamlPlatform.Avalonia, XamlPlatform.Maui, XamlPlatform.UWP, XamlPlatform.XF];

    /// <summary>
    /// Optimizes the specified xaml document.
    /// </summary>
    /// <param name="xamlDocument">The xaml document.</param>
    /// <param name="xamlFile">The xaml file.</param>
    /// <returns>The optimization result.</returns>
    public OptimizationResult Optimize(XDocument xamlDocument, IFileReference xamlFile)
    {
        if (xamlDocument.Root == null)
        {
            return OptimizationResult.None();
        }

        var hasBeenOptimized = false;
        foreach (var xElement in xamlDocument.Root.DescendantsAndSelf())
        {
            foreach (var attribute in xElement.Attributes())
            {
                const string dynamicResource = "{DynamicResource ";
                if (attribute.Value.StartsWith(StaticResourcePrefix) && attribute.Value.AsSpan(StaticResourcePrefix.Length).Contains(this.staticToDynamicResourceSettings.DynamicMarker.AsSpan(), StringComparison.InvariantCulture) && !this.IsDefinedIsSameDocument(attribute.Value, xamlDocument.Root))
                {
                    attribute.Value = dynamicResource + attribute.Value.AsSpan(StaticResourcePrefix.Length).ToString();
                    hasBeenOptimized = true;
                }
            }
        }

        return OptimizationResult.From(hasBeenOptimized, xamlDocument);
    }

    private bool IsDefinedIsSameDocument(string attributeValue, XElement rootElement)
    {
        var resourceKey = attributeValue.AsSpan(StaticResourcePrefix.Length, attributeValue.Length - StaticResourcePrefix.Length - 1);
        foreach (var element in rootElement.DescendantsAndSelf())
        {
            var keyAttribute = element.Attribute(this.keyName);
            if (keyAttribute != null && keyAttribute.Value.AsSpan().Equals(resourceKey, StringComparison.InvariantCulture))
            {
                return true;
            }
        }

        return false;
    }
}
