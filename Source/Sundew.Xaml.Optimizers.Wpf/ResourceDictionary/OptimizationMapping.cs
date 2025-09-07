// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationMapping.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary;

/// <summary>
/// Represents a mapping for optimizations in the resource dictionary caching settings.
/// </summary>
public class OptimizationMapping
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizationMapping"/> class.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="action">The optimization.</param>
    /// <param name="replacementType">The xaml type.</param>
    public OptimizationMapping(string category, OptimizationAction action, string? replacementType = null)
    {
        this.Category = category;
        this.Action = action;
        this.ReplacementType = replacementType;
    }

    /// <summary>
    /// Gets the category of the optimization mapping.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the optimization type.
    /// </summary>
    public OptimizationAction Action { get; }

    /// <summary>
    /// Gets the XAML type associated with the optimization mapping.
    /// </summary>
    public string? ReplacementType { get; }
}