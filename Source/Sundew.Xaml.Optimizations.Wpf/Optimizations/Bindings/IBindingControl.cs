// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingControl.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

/// <summary>
/// An interface for implementing a binding control.
/// </summary>
public interface IBindingControl
{
    /// <summary>
    /// Gets or sets a value indicating whether an update is scheduled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is queued; otherwise, <c>false</c>.
    /// </value>
    bool IsUpdatePending { get; set; }

    /// <summary>
    /// Updates the target value.
    /// </summary>
    void UpdateTargetValue();
}