// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionary.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations;

using System.Windows;
using Sundew.Xaml.Theming;
using SystemResourceDictionary = System.Windows.ResourceDictionary;

/// <summary>
/// A ResourceDictionary that ensures that a source is only loaded once and otherwise retrieved from a cache.
/// </summary>
/// <seealso cref="SystemResourceDictionary" />
public sealed class ResourceDictionary : ResourceDictionaryBase<ResourceDictionary>
{
    /// <summary>
    /// Identifies the Category dependency property.
    /// </summary>
    public static readonly DependencyProperty CategoryProperty = DependencyProperty.RegisterAttached(
        "Category", typeof(string), typeof(ResourceDictionary), new PropertyMetadata(default(string)));

    /// <summary>
    /// Sets the category for the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The category.</param>
    public static void SetCategory(DependencyObject element, string? value)
    {
        element.SetValue(CategoryProperty, value);
    }

    /// <summary>
    /// Gets the category for the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The category.</returns>
    public static string? GetCategory(DependencyObject element)
    {
        return (string?)element.GetValue(CategoryProperty);
    }
}