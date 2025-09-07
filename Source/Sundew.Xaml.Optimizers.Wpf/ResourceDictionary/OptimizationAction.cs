// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationAction.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary;

/// <summary>
/// Represents the optimization mode for resource dictionary caching.
/// </summary>
public enum OptimizationAction
{
    /// <summary>
    /// No optimization is done.
    /// </summary>
    None,

    /// <summary>
    /// The optimization is to remove the dictionary.
    /// </summary>
    Remove,

    /// <summary>
    /// The optimization is to replace the dictionary.
    /// </summary>
    Replace,
}