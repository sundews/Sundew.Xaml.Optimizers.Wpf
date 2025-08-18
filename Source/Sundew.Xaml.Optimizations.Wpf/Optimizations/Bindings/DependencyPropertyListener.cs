// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyListener.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;

/// <summary>Registers to dependency property change notifications.</summary>
public sealed class DependencyPropertyListener : IDisposable
{
    private DependencyPropertyDescriptor dependencyPropertyDescriptor;
    private DependencyObject dependencyObject;
    private EventHandler eventHandler;

    private DependencyPropertyListener(DependencyPropertyDescriptor dependencyPropertyDescriptor, DependencyObject dependencyObject, EventHandler eventHandler)
    {
        this.dependencyPropertyDescriptor = dependencyPropertyDescriptor;
        this.dependencyObject = dependencyObject;
        this.eventHandler = eventHandler;
        this.dependencyPropertyDescriptor.AddValueChanged(dependencyObject, eventHandler);
    }

    /// <summary>Subscribes the specified dependency object.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="dependencyProperty">The dependency property.</param>
    /// <param name="eventHandler">The event handler.</param>
    public static DependencyPropertyListener Subscribe(DependencyObject dependencyObject, DependencyProperty dependencyProperty, EventHandler eventHandler)
    {
        var dependencyPropertyDescriptor =
            dependencyProperty.OwnerType.IsAbstract && dependencyProperty.OwnerType.IsSealed
                ? null
                : DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyProperty.OwnerType);
        if (dependencyPropertyDescriptor == null)
        {
            dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, dependencyObject.GetType());
        }

        return new DependencyPropertyListener(dependencyPropertyDescriptor, dependencyObject, eventHandler);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.dependencyPropertyDescriptor.RemoveValueChanged(this.dependencyObject, this.eventHandler);
        this.dependencyObject = null;
        this.eventHandler = null;
        this.dependencyPropertyDescriptor = null;
    }
}