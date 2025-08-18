// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationMode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

/// <summary>
/// Determines how resource dictionaries should be optimized.
/// </summary>
internal enum OptimizationMode
{
    /// <summary>
    /// Indicates no optimization should be applied.
    /// </summary>
    TVoid,

    /// <summary>
    /// Indicates that shared resource dictionary optimization should be applied.
    /// </summary>
    Shared,
}