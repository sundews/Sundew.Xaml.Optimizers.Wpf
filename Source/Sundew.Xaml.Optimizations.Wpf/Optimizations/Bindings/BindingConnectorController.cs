// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingConnectorController.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.Windows;
using System.Windows.Interop;

/// <summary>Controller for <see cref="IBindingConnector"/>.</summary>
/// <seealso cref="System.IDisposable" />
public class BindingConnectorController
{
    private readonly IBindingConnector bindingConnector;

    /// <summary>Initializes a new instance of the <see cref="BindingConnectorController"/> class.</summary>
    /// <param name="bindingConnector">The binding connector.</param>
    public BindingConnectorController(IBindingConnector bindingConnector)
    {
        this.bindingConnector = bindingConnector;
    }

    /// <summary>Initializes the specified root.</summary>
    /// <param name="root">The root.</param>
    public void Initialize(DependencyObject root)
    {
        if (root is FrameworkElement frameworkElement)
        {
            frameworkElement.DataContextChanged += this.OnFrameworkElementDataContextChanged;
            frameworkElement.Unloaded += this.OnFrameworkElementUnloaded;
        }

#if WPF
        if (root is FrameworkContentElement frameworkContentElement)
        {
            frameworkContentElement.DataContextChanged += this.OnFrameworkElementDataContextChanged;
            frameworkContentElement.Unloaded += this.OnFrameworkElementUnloaded;
        }
#endif

        if (root.Dispatcher != null)
        {
            root.Dispatcher.ShutdownFinished += this.OnDispatcherShutdownFinished;
        }

        if (PresentationSource.FromDependencyObject(root) is HwndSource hwndSource)
        {
            hwndSource.Disposed += this.OnHwndSourceDisposed;
        }
    }

    /// <summary>Uninitializes the specified root.</summary>
    /// <param name="root">The root.</param>
    public void UnInitialize(DependencyObject root)
    {
        if (root is FrameworkElement frameworkElement)
        {
            frameworkElement.DataContextChanged -= this.OnFrameworkElementDataContextChanged;
            frameworkElement.Unloaded -= this.OnFrameworkElementUnloaded;
        }

        if (root is FrameworkContentElement frameworkContentElement)
        {
            frameworkContentElement.DataContextChanged -= this.OnFrameworkElementDataContextChanged;
            frameworkContentElement.Unloaded -= this.OnFrameworkElementUnloaded;
        }

        if (PresentationSource.FromDependencyObject(root) is HwndSource hwndSource)
        {
            hwndSource.Disposed -= this.OnHwndSourceDisposed;
        }
    }

    private void OnFrameworkElementLoaded(object sender, RoutedEventArgs e)
    {
        if (this.bindingConnector.Root is FrameworkElement frameworkElement)
        {
            frameworkElement.Loaded -= this.OnFrameworkElementLoaded;
        }

        if (this.bindingConnector.Root is FrameworkContentElement frameworkContentElement)
        {
            frameworkContentElement.Loaded -= this.OnFrameworkElementLoaded;
        }

        this.bindingConnector.Reconnect();
    }

    private void OnFrameworkElementDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        this.bindingConnector.Refresh();
    }

    private void OnHwndSourceDisposed(object sender, EventArgs e)
    {
        this.bindingConnector.Dispose();
    }

    private void OnDispatcherShutdownFinished(object sender, EventArgs e)
    {
        this.bindingConnector.Dispose();
    }

    private void OnFrameworkElementUnloaded(object sender, RoutedEventArgs e)
    {
        this.bindingConnector.Disconnect();
        if (this.bindingConnector.Root is FrameworkElement frameworkElement)
        {
            frameworkElement.Loaded += this.OnFrameworkElementLoaded;
        }

        if (this.bindingConnector.Root is FrameworkContentElement frameworkContentElement)
        {
            frameworkContentElement.Loaded += this.OnFrameworkElementLoaded;
        }
    }
}