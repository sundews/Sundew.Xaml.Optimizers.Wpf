// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBinding{TRoot,TSource,TTarget,TValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.Runtime.CompilerServices;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

/// <summary>Acts as a destination of a binding.</summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <typeparam name="TValue">The type of the target value.</typeparam>
public class DataBinding<TRoot, TSource, TTarget, TValue> : IBinding, ITwoWayBindingControl<TValue, TValue>
    where TTarget : DependencyObject
{
    private readonly INotifyingProperty<TSource> sourceProperty;
    private readonly Action<TSource, TValue> setSource;
    private readonly int phase;
    private DataBindingState<TRoot, TSource, TValue, TTarget, TValue> dataBindingState;
    private DependencyPropertyListener dependencyPropertyListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBinding{TRoot,TSource,TTarget,TValue}" /> class.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="bindingContext">The data context.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="setSource">The set source.</param>
    /// <param name="updateSourceTrigger">The update source trigger value.</param>
    /// <param name="bindingMode">The binding mode value.</param>
    /// <param name="phase">The phase.</param>
    public DataBinding(
        int id,
        IBindingContext<TRoot, TSource> bindingContext,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TValue> getTargetValue,
        Action<TSource, TValue> setSource,
        UpdateSourceTrigger updateSourceTrigger,
        BindingMode bindingMode,
        int phase)
    {
        this.sourceProperty = sourceProperty;
        this.setSource = setSource;
        DataBindingHelper.GetNotificationParameters(target, targetProperty, ref bindingMode, ref updateSourceTrigger);
        DataBindingHelper.TryRegisterLostFocus(target, updateSourceTrigger, this.OnTargetLostFocus);
        var conversionParameters = ConversionProvider.GetConversionParameters<TValue, TValue>(target, id);
        this.dataBindingState = new DataBindingState<TRoot, TSource, TValue, TTarget, TValue>(bindingContext, target, targetProperty, getTargetValue, getSourceValue, bindingMode, updateSourceTrigger, conversionParameters);
        this.sourceProperty.Initialize(this);
        this.phase = phase;
    }

    bool IBindingControl.IsUpdatePending
    {
        get => this.dataBindingState.IsUpdatePending;
        set => this.dataBindingState.IsUpdatePending = value;
    }

    /// <summary>
    /// Connects this instance.
    /// </summary>
    public void Connect(int currentPhase)
    {
        if (this.phase != currentPhase)
        {
            return;
        }

        if (this.dataBindingState.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
        {
            this.dependencyPropertyListener = DependencyPropertyListener.Subscribe(this.dataBindingState.Target, this.dataBindingState.TargetProperty, this.OnTargetPropertyChanged);
        }

        this.TryAttachToPropertyChangeNotification();
        DataBindingHelper.Refresh(this.dataBindingState.BindingMode, this);
    }

    /// <summary>Refreshes this instance.</summary>
    public void Refresh()
    {
        this.TryAttachToPropertyChangeNotification();
        if (this.dataBindingState.BindingMode != BindingMode.OneTime)
        {
            DataBindingHelper.Refresh(this.dataBindingState.BindingMode, this);
        }
    }

    /// <summary>
    /// Disconnects this instance.
    /// </summary>
    public void Disconnect()
    {
        this.sourceProperty.Unsubscribe(this.dataBindingState.CurrentSource);
        this.dependencyPropertyListener?.Dispose();
        this.dependencyPropertyListener = null;
        this.dataBindingState.CurrentSource = default;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.Disconnect();
        this.dataBindingState = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TValue IBindingControl<TValue, TValue>.Convert(TValue sourceValue)
    {
        return sourceValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IBindingControl.UpdateTargetValue()
    {
        this.UpdateTargetValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TValue ITwoWayBindingControl<TValue, TValue>.GetSourceValue(TValue targetValue)
    {
        return targetValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ITwoWayBindingControl<TValue, TValue>.UpdateSourceValue()
    {
        this.UpdateSourceValue();
    }

    private void TryAttachToPropertyChangeNotification()
    {
        this.dataBindingState.CurrentSource = this.sourceProperty.TryUpdateSubscription(this.dataBindingState.BindingMode, this.dataBindingState.CurrentSource);
    }

    private void OnTargetLostFocus(object sender, RoutedEventArgs e)
    {
        this.UpdateSourceValue();
    }

    private void OnTargetPropertyChanged(object sender, EventArgs e)
    {
        this.UpdateSourceValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateSourceValue()
    {
        DataBindingHelper.UpdateSourceValue(this, ref this.dataBindingState, this.setSource);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTargetValue()
    {
        DataBindingHelper.UpdateTargetValue(this, this.dataBindingState);
    }
}