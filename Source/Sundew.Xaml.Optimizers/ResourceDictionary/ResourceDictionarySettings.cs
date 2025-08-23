// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionarySettings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary;

using System.Collections.Generic;

/// <summary>
/// Used to deserialize settings for the <see cref="ResourceDictionaryOptimizer"/>.
/// </summary>
public class ResourceDictionarySettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceDictionarySettings"/> class.
    /// </summary>
    /// <param name="optimizationMappings">The optimization mappings.</param>
    /// <param name="replaceUncategorized">The replace uncategorized.</param>
    /// <param name="defaultReplacementType">The default replacement type.</param>
    public ResourceDictionarySettings(IReadOnlyList<OptimizationMapping> optimizationMappings, bool? replaceUncategorized = null, string? defaultReplacementType = null)
    {
        this.OptimizationMappings = optimizationMappings;
        this.ReplaceUncategorized = replaceUncategorized;
        this.DefaultReplacementType = defaultReplacementType;
    }

    /// <summary>
    /// Gets the default replacement type for resource dictionaries that do not have a specific optimization mapping.
    /// </summary>
    public string? DefaultReplacementType { get; }

    /// <summary>
    /// Gets the optimization mappings.
    /// </summary>
    public IReadOnlyList<OptimizationMapping> OptimizationMappings { get; }

    /// <summary>
    /// Gets a value indicating whether uncategorized resource dictionaries should be replaced with the default replacement type.
    /// </summary>
    public bool? ReplaceUncategorized { get; }
}