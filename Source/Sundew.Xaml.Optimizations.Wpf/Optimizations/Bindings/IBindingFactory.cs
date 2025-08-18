// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;

#endif

/// <summary>
/// Interface for building a binding factory.
/// </summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <typeparam name="TSource">The type of the source.</typeparam>
public interface IBindingFactory<TRoot, TSource> : ISourceNotificationFactory<TSource>
    where TRoot : DependencyObject
{
    /// <summary>Creates the data context.</summary>
    /// <typeparam name="TElement">The type of the new element.</typeparam>
    /// <typeparam name="TNewSource">The type of the new source.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="newElement">The new element.</param>
    /// <param name="getDataContext">The get data context.</param>
    /// <param name="propertyName">The sourceProperty name.</param>
    /// <returns>A new <see cref="ChildBindingContext{TRoot,TElement,TNewSource}"/>.</returns>
    ChildBindingContext<TRoot, TElement, TNewSource> BindTargetDataContextOneWay<TElement, TNewSource>(
        int id,
        TElement newElement,
        Func<TSource, TNewSource> getDataContext,
        string propertyName)
        where TElement : DependencyObject;

    /// <summary>Binds to data context one way.</summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TTargetValue">The type of the target value.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="target">The target.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <returns>A new <see cref="DataContextBinding{TRoot, TSource, TTarget, TTargetValue}"/>.</returns>
    DataContextBinding<TRoot, TSource, TTarget, TTargetValue> BindSourceDataContextOneWay<TTarget, TTargetValue>(
        int id,
        TTarget target,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue)
        where TTarget : DependencyObject;

    /// <summary>
    /// Binds the specified source property.
    /// </summary>
    /// <typeparam name="TNewSource">The type of the new source.</typeparam>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourcePart">The get source.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>
    /// A path part.
    /// </returns>
    PathPart<TRoot, TSource, TNewSource> BindPart<TNewSource>(
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TNewSource> getSourcePart,
        BindingMode bindingMode,
        int phase = 0);

    /// <summary>
    /// Destinations the specified get target.
    /// </summary>
    /// <typeparam name="TSourceValue">The type of the source value.</typeparam>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TTargetValue">The type of the target value.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">the target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>
    /// A new destination.
    /// </returns>
    DataBindingOneWay<TRoot, TSource, TSourceValue, TTarget, TTargetValue> BindOneWay<TSourceValue, TTarget, TTargetValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TSourceValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject;

    /// <summary>
    /// Destinations the specified get target.
    /// </summary>
    /// <typeparam name="TSourceValue">The type of the source value.</typeparam>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TTargetValue">The type of the target value.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="setSourceValue">The set source value.</param>
    /// <param name="updateSourceTrigger">The update source trigger.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>
    /// A new destination.
    /// </returns>
    DataBinding<TRoot, TSource, TSourceValue, TTarget, TTargetValue> Bind<TSourceValue, TTarget, TTargetValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TSourceValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue,
        Action<TSource, TSourceValue> setSourceValue,
        UpdateSourceTrigger updateSourceTrigger,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject;

    /// <summary>
    /// Destinations the specified get target.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TValue">The type of the target value.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">the target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>
    /// A new destination.
    /// </returns>
    DataBindingOneWay<TRoot, TSource, TTarget, TValue> BindInvariantOneWay<TTarget, TValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TValue> getTargetValue,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject;

    /// <summary>
    /// Destinations the specified get target.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TValue">The type of the target value.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="setSourceValue">The set source value.</param>
    /// <param name="updateSourceTrigger">The update source trigger.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    /// <returns>
    /// A new destination.
    /// </returns>
    DataBinding<TRoot, TSource, TTarget, TValue> BindInvariant<TTarget, TValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TValue> getTargetValue,
        Action<TSource, TValue> setSourceValue,
        UpdateSourceTrigger updateSourceTrigger,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject;
}