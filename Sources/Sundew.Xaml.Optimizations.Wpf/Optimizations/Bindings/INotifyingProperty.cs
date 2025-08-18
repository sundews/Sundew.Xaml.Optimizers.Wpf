// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyingProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif

/// <summary>Interface for implementing a notifying property.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
public interface INotifyingProperty<TSource>
{
    /// <summary>
    /// Initializes the notififying property.
    /// </summary>
    /// <param name="bindingControl">The binding control.</param>
    void Initialize(IBindingControl bindingControl);

    /// <summary>Tries the update subscription.</summary>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="source">The source.</param>
    /// <returns>The new source.</returns>
    TSource TryUpdateSubscription(BindingMode bindingMode, TSource source);

    /// <summary>Unsubscribes.</summary>
    void Unsubscribe(TSource source);
}