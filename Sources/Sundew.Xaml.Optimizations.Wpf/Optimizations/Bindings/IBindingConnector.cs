// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingConnector.cs" company="Sundews">
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

/// <summary>Interface for implementing a binding connector.</summary>
public interface IBindingConnector : IDisposable
{
    /// <summary>
    /// Gets the root.
    /// </summary>
    /// <value>
    /// The root.
    /// </value>
    DependencyObject Root { get; }

    /// <summary>Connects the specified target dependency object.</summary>
    /// <param name="root">The target dependency object.</param>
    void Connect(DependencyObject root);

    /// <summary>Refreshes this instance.</summary>
    void Refresh();

    /// <summary>Disconnects this instance.</summary>
    void Disconnect();

    /// <summary>
    /// Reconnects this instance.
    /// </summary>
    void Reconnect();
}