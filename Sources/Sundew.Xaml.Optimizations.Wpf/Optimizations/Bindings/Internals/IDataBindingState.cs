// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataBindingState.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
using System.Windows;
#endif

/// <summary>Contains the binding state.</summary>
public interface IDataBindingState
{
    /// <summary>Gets the target.</summary>
    /// <value>The target.</value>
    DependencyObject Target { get; }

    /// <summary>Gets the target property.</summary>
    /// <value>The target property.</value>
    DependencyProperty TargetProperty { get; }

    /// <summary>Gets or sets a value indicating whether this instance is updating.</summary>
    /// <value>
    ///   <c>true</c> if this instance is updating; otherwise, <c>false</c>.</value>
    bool IsUpdating { get; set; }
}