// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingContext{TRoot,TSource}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.Collections.Generic;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using Dispatcher = Windows.UI.Core.CoreDispatcher;
#else
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
#endif
using Sundew.Xaml.Optimizations.Bindings.Internals;

/// <summary>Acts as a source of a binding.</summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <typeparam name="TSource">The type of the source.</typeparam>
public class BindingContext<TRoot, TSource> : IBindingContext<TRoot, TSource>, IBinding, IBindingFactory<TRoot, TSource>
    where TRoot : DependencyObject
{
    private readonly List<IBinding> bindings = new List<IBinding>();
    private readonly Func<TRoot, TSource> getSource;
    private readonly bool canSourceChange;

    internal BindingContext(TRoot root, Func<TRoot, TSource> getSource, bool canSourceChange)
    {
        this.Root = root;
        this.getSource = getSource;
        this.canSourceChange = canSourceChange;
        this.Engine = DataBindingHelper.EnsureInitialized(this.Root.Dispatcher);
    }

    /// <summary>Gets the source.</summary>
    /// <value>The source.</value>
    public TSource Source { get; private set; }

    /// <summary>Gets the engine.</summary>
    /// <value>The engine.</value>
    public Engine Engine { get; }

    /// <summary>Gets the root.</summary>
    /// <value>The root.</value>
    public TRoot Root { get; }

    /// <summary>Creates the source property.</summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>A new <see cref="INotifyingProperty{TSource}"/>.</returns>
    public INotifyingProperty<TSource> CreateSourceProperty(string propertyName)
    {
        return new PropertyChangedNotifyingProperty<TSource>(this, propertyName);
    }

    /// <summary>Creates the source property.</summary>
    /// <param name="dependencyProperty">The dependency property.</param>
    /// <returns>A new <see cref="INotifyingProperty{TSource}"/>.</returns>
    public INotifyingProperty<TSource> CreateSourceProperty(DependencyProperty dependencyProperty)
    {
        return new DependencyNotifyingProperty<TSource>(this, dependencyProperty);
    }

    /// <summary>Creates the source property.</summary>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="subscribe">The subscribe function.</param>
    /// <param name="unsubscribe">The unsubscribe function.</param>
    /// <returns>A new <see cref="INotifyingProperty{TSource}"/>.</returns>
    public INotifyingProperty<TSource> CreateSourceProperty<TEventHandler>(
        Func<TSource, Action, TEventHandler> subscribe,
        Action<TSource, TEventHandler> unsubscribe)
    {
        return new EventNotifyingProperty<TSource, TEventHandler>(this, subscribe, unsubscribe);
    }

    /// <summary>Creates the data context.</summary>
    /// <typeparam name="TElement">The type of the new element.</typeparam>
    /// <typeparam name="TNewSource">The type of the new source.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="newElement">The new root.</param>
    /// <param name="getDataContext">The get data context.</param>
    /// <param name="propertyName">The sourceProperty name.</param>
    /// <returns>A new <see cref="ChildBindingContext{TRoot,TElement,TNewSource}"/>.</returns>
    public ChildBindingContext<TRoot, TElement, TNewSource> BindTargetDataContextOneWay<TElement, TNewSource>(
        int id,
        TElement newElement,
        Func<TSource, TNewSource> getDataContext,
        string propertyName)
        where TElement : DependencyObject
    {
        var childDataContext = new ChildBindingContext<TRoot, TElement, TNewSource>(
            this.Root,
            newElement,
            frameworkElement => getDataContext(this.Source),
            propertyName,
            true);

        this.bindings.Add(childDataContext);
        return childDataContext;
    }

    /// <summary>Binds to data context one way.</summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TTargetValue">The type of the target value.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="target">The target.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <returns>A new <see cref="DataContextBinding{TRoot, TSource, TTarget, TTargetValue}"/>.</returns>
    public DataContextBinding<TRoot, TSource, TTarget, TTargetValue> BindSourceDataContextOneWay<TTarget, TTargetValue>(
        int id,
        TTarget target,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue)
        where TTarget : DependencyObject
    {
        var dataContextBinding = new DataContextBinding<TRoot, TSource, TTarget, TTargetValue>(id, this, target, targetProperty, getTargetValue);
        this.bindings.Add(dataContextBinding);
        return dataContextBinding;
    }

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
    public PathPart<TRoot, TSource, TNewSource> BindPart<TNewSource>(
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TNewSource> getSourcePart,
        BindingMode bindingMode,
        int phase = 0)
    {
        var pathPart = new PathPart<TRoot, TSource, TNewSource>(this, sourceProperty, getSourcePart, bindingMode, phase);
        this.bindings.Add(pathPart);
        return pathPart;
    }

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
    public DataBindingOneWay<TRoot, TSource, TSourceValue, TTarget, TTargetValue> BindOneWay<TSourceValue, TTarget, TTargetValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TSourceValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject
    {
        var dataDestination = new DataBindingOneWay<TRoot, TSource, TSourceValue, TTarget, TTargetValue>(
            id,
            this,
            target,
            sourceProperty,
            getSourceValue,
            targetProperty,
            getTargetValue,
            bindingMode,
            phase);
        this.bindings.Add(dataDestination);
        return dataDestination;
    }

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
    public DataBinding<TRoot, TSource, TSourceValue, TTarget, TTargetValue> Bind<TSourceValue, TTarget, TTargetValue>(
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
        where TTarget : DependencyObject
    {
        var dataDestination = new DataBinding<TRoot, TSource, TSourceValue, TTarget, TTargetValue>(
            id,
            this,
            target,
            sourceProperty,
            getSourceValue,
            targetProperty,
            getTargetValue,
            setSourceValue,
            updateSourceTrigger,
            bindingMode,
            phase);
        this.bindings.Add(dataDestination);
        return dataDestination;
    }

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
    public DataBindingOneWay<TRoot, TSource, TTarget, TValue> BindInvariantOneWay<TTarget, TValue>(
        int id,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TValue> getTargetValue,
        BindingMode bindingMode,
        int phase = 0)
        where TTarget : DependencyObject
    {
        var dataDestination = new DataBindingOneWay<TRoot, TSource, TTarget, TValue>(
            id,
            this,
            target,
            sourceProperty,
            getSourceValue,
            targetProperty,
            getTargetValue,
            bindingMode,
            phase);
        this.bindings.Add(dataDestination);
        return dataDestination;
    }

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
    public DataBinding<TRoot, TSource, TTarget, TValue> BindInvariant<TTarget, TValue>(
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
        where TTarget : DependencyObject
    {
        var dataDestination = new DataBinding<TRoot, TSource, TTarget, TValue>(
            id,
            this,
            target,
            sourceProperty,
            getSourceValue,
            targetProperty,
            getTargetValue,
            setSourceValue,
            updateSourceTrigger,
            bindingMode,
            phase);
        this.bindings.Add(dataDestination);
        return dataDestination;
    }

    /// <summary>
    /// Connects this instance.
    /// </summary>
    public void Connect(int phase)
    {
        if (phase == 0)
        {
            this.Source = this.GetSource();
        }

        foreach (var binding in this.bindings)
        {
            binding.Connect(phase);
        }
    }

    /// <summary>Refreshes this instance.</summary>
    public virtual void Refresh()
    {
        if (this.canSourceChange)
        {
            var oldSource = this.Source;
            var newSource = this.GetSource();
            if (!Equals(oldSource, newSource))
            {
                this.Source = newSource;
                this.OnSourceChanged(oldSource);
            }
        }

        foreach (var binding in this.bindings)
        {
            binding.Refresh();
        }
    }

    /// <summary>
    /// Disconnects this instance.
    /// </summary>
    public void Disconnect()
    {
        foreach (var binding in this.bindings)
        {
            binding.Disconnect();
        }

        this.Source = default;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.Disconnect();
        this.Dispose(true);
    }

    /// <summary>Occurs when the source has changed.</summary>
    protected virtual void OnSourceChanged(TSource oldSource)
    {
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var binding in this.bindings)
            {
                binding.Dispose();
            }

            this.bindings.Clear();
        }
    }

    private TSource GetSource()
    {
        try
        {
            return this.getSource(this.Root);
        }
        catch (Exception)
        {
            return default;
        }
    }
}