// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticToDynamicResourceSettings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.StaticToDynamicResource;

using Sundew.Xaml.Optimization;

/// <summary>
/// Settings for the <see cref="StaticToDynamicResourceOptimizer"/>.
/// </summary>
public class StaticToDynamicResourceSettings : OptimizerSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticToDynamicResourceSettings"/> class with the specified dynamic marker.
    /// </summary>
    /// <param name="dynamicMarker">The dynamic marker.</param>
    /// <param name="debug">A value indicating whether the optimizer should be debugged.</param>
    public StaticToDynamicResourceSettings(string? dynamicMarker = null, bool debug = false)
     : base(debug)
    {
        this.DynamicMarker = dynamicMarker ?? "🔄️";
    }

    /// <summary>
    /// Gets the marker that indicates that a resource should be converted to a dynamic resource.
    /// </summary>
    public string DynamicMarker { get; }
}