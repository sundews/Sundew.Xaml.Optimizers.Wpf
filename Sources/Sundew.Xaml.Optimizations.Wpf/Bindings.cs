// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bindings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
using System.Windows;
#endif

/// <summary>Contains the data type attached dependency property.</summary>
public static class Bindings
{
    /// <summary>The data type property.</summary>
    public static readonly DependencyProperty DataTypeProperty = DependencyProperty.RegisterAttached(
        "DataType", typeof(Type), typeof(Bindings), new PropertyMetadata(default));

    /// <summary>The data type property.</summary>
    public static readonly DependencyProperty OptimizeBindingsProperty = DependencyProperty.RegisterAttached(
        "OptimizeBindings", typeof(bool), typeof(Bindings), new PropertyMetadata(default));

    /// <summary>The phase property.</summary>
    public static readonly DependencyProperty PhaseProperty = DependencyProperty.RegisterAttached(
        "Phase", typeof(int), typeof(Bindings), new PropertyMetadata(0));

    /// <summary>Gets the type of the data.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A type.</returns>
    public static Type GetDataType(DependencyObject dependencyObject)
    {
        return (Type)dependencyObject.GetValue(DataTypeProperty);
    }

    /// <summary>Sets the type of the data.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetDataType(DependencyObject dependencyObject, Type value)
    {
        dependencyObject.SetValue(DataTypeProperty, value);
    }

    /// <summary>Gets the optimize bindings value.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>A type.</returns>
    public static bool GetOptimizeBindings(DependencyObject dependencyObject)
    {
        return (bool)dependencyObject.GetValue(OptimizeBindingsProperty);
    }

    /// <summary>Sets the optimize bindings value.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetOptimizeBindings(DependencyObject dependencyObject, Type value)
    {
        dependencyObject.SetValue(OptimizeBindingsProperty, value);
    }

    /// <summary>Gets the phase.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <returns>The phase.</returns>
    public static int GetPhase(DependencyObject dependencyObject)
    {
        return (int)dependencyObject.GetValue(PhaseProperty);
    }

    /// <summary>Sets the phase.</summary>
    /// <param name="dependencyObject">The dependency object.</param>
    /// <param name="value">The value.</param>
    public static void SetPhase(DependencyObject dependencyObject, int value)
    {
        dependencyObject.SetValue(PhaseProperty, value);
    }
}