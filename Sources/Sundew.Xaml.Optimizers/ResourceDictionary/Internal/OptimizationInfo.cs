// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

/// <summary>
/// Info about how a resource dictionary should be optimized.
/// </summary>
internal sealed class OptimizationInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizationInfo"/> class.
    /// </summary>
    /// <param name="optimizationMode">The optimization mode.</param>
    /// <param name="source">The binding.</param>
    public OptimizationInfo(OptimizationMode optimizationMode, string? source)
    {
        this.OptimizationMode = optimizationMode;
        this.Source = source;
    }

    /// <summary>
    /// Gets the optimization mode.
    /// </summary>
    /// <value>
    /// The optimization mode.
    /// </value>
    public OptimizationMode OptimizationMode { get; }

    /// <summary>
    /// Gets the binding.
    /// </summary>
    /// <value>
    /// The binding.
    /// </value>
    public string? Source { get; }
}