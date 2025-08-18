// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISourceNotificationFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
using System.Windows;
#endif

/// <summary>
/// Interface for implementing a source notification binder.
/// </summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
public interface ISourceNotificationFactory<TSource>
{
    /// <summary>
    /// Creates a notifying property for a dependency property.
    /// </summary>
    /// <param name="dependencyProperty">The dependency property.</param>
    /// <returns>The <see cref="INotifyingProperty{TSource}"/>.</returns>
    INotifyingProperty<TSource> CreateSourceProperty(DependencyProperty dependencyProperty);

    /// <summary>
    /// Creates a notifying property for a property.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The <see cref="INotifyingProperty{TSource}"/>.</returns>
    INotifyingProperty<TSource> CreateSourceProperty(string propertyName);

    /// <summary>
    /// Creates a notifying property for an event.
    /// </summary>
    /// <param name="subscribe">The subscribe action.</param>
    /// <param name="unsubscribe">The unsubscribe action.</param>
    /// <returns>The <see cref="INotifyingProperty{TSource}"/>.</returns>
    INotifyingProperty<TSource> CreateSourceProperty<TEventHandler>(Func<TSource, Action, TEventHandler> subscribe, Action<TSource, TEventHandler> unsubscribe);
}