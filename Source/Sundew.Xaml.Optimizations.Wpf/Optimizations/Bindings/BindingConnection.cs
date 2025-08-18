// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingConnection.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

#if WINDOWS_UWP
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;
#else
using System.ComponentModel;
using System.Windows;

#endif

/// <summary>Contains attached dependency property for connecting compiled bindings.</summary>
public class BindingConnection
{
    /// <summary>The binding connector property.</summary>
    public static readonly DependencyProperty BindingConnectorProperty = DependencyProperty.RegisterAttached(
        "BindingConnector",
        typeof(IBindingConnector),
        typeof(BindingConnection),
        new PropertyMetadata(null, OnBindingConnectorChanged));

    /// <summary>The binding connector property.</summary>
    public static readonly DependencyProperty MetadataProperty = DependencyProperty.RegisterAttached(
        "Metadata",
        typeof(BindingData),
        typeof(BindingConnection),
        new PropertyMetadata(null));

    /// <summary>Gets the binding connector.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A binding connector.</returns>
    public static IBindingConnector GetBindingConnector(DependencyObject dependencyObject)
    {
        return (IBindingConnector)dependencyObject.GetValue(BindingConnectorProperty);
    }

    /// <summary>Sets the binding connector.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The binding connector.</param>
    public static void SetBindingConnector(DependencyObject dependencyObject, IBindingConnector value)
    {
        dependencyObject.SetValue(BindingConnectorProperty, value);
    }

    /// <summary>Gets the binding dictionary.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A binding connector.</returns>
    public static BindingData GetMetadata(DependencyObject dependencyObject)
    {
        return (BindingData)dependencyObject.GetValue(MetadataProperty);
    }

    /// <summary>Sets the binding dictionary.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The binding connector.</param>
    public static void SetMetadata(DependencyObject dependencyObject, BindingData value)
    {
        dependencyObject.SetValue(MetadataProperty, value);
    }

    private static void OnBindingConnectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
#if WINDOWS_UWP
            if (DesignMode.DesignMode2Enabled || DesignMode.DesignModeEnabled)
            {
                return;
            }
#else
        if (DesignerProperties.GetIsInDesignMode(d))
        {
            return;
        }
#endif
        if (e.OldValue is IBindingConnector oldBindingConnector)
        {
            oldBindingConnector.Disconnect();
        }

        if (e.NewValue is IBindingConnector newBindingConnector)
        {
            newBindingConnector.Connect(d);
        }
    }
}