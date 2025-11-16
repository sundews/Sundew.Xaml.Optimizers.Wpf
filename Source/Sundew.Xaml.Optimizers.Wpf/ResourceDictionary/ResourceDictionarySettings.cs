// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionarySettings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary;

using System.Collections.Generic;
using Sundew.Xaml.Optimization;

/// <summary>
/// Used to deserialize settings for the <see cref="ResourceDictionaryOptimizer"/>.
/// </summary>
public class ResourceDictionarySettings : OptimizerSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceDictionarySettings"/> class.
    /// </summary>
    /// <param name="optimizationMappings">The optimization mappings.</param>
    /// <param name="replaceUncategorized">A value indicating whether uncategorized should be replaced.</param>
    /// <param name="defaultReplacementType">The default replacement type.</param>
    /// <param name="debug">A value indicating whether the optimizer should be debugged.</param>
    public ResourceDictionarySettings(IReadOnlyList<OptimizationMapping> optimizationMappings, bool? replaceUncategorized = null, string? defaultReplacementType = null, bool debug = false)
     : base(debug)
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