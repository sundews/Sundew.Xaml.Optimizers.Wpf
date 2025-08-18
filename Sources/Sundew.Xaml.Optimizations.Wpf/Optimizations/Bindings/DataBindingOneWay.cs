// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBindingOneWay.cs" company="Sundews">
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
/// <typeparam name="TSourceValue">The type of the source value.</typeparam>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <typeparam name="TTargetValue">The type of the target value.</typeparam>
public class DataBindingOneWay<TRoot, TSource, TSourceValue, TTarget, TTargetValue> : IBinding, IBindingControl<TSourceValue, TTargetValue>
    where TTarget : DependencyObject
{
    private readonly INotifyingProperty<TSource> sourceProperty;
    private readonly int phase;
    private DataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue> dataBindingState;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataBindingOneWay{TRoot, TSource, TSourceValue, TTarget, TTargetValue}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="bindingContext">The binding context.</param>
    /// <param name="target">The target.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="getSourceValue">The get source value.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    /// <param name="bindingMode">The binding mode.</param>
    /// <param name="phase">The phase.</param>
    public DataBindingOneWay(
        int id,
        IBindingContext<TRoot, TSource> bindingContext,
        TTarget target,
        INotifyingProperty<TSource> sourceProperty,
        Func<TSource, TSourceValue> getSourceValue,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue,
        BindingMode bindingMode,
        int phase)
    {
        this.sourceProperty = sourceProperty;
        this.phase = phase;
        var conversionParameters = ConversionProvider.GetConversionParameters<TSourceValue, TTargetValue>(target, id, bindingMode);
        this.dataBindingState = new DataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue>(
            bindingContext,
            target,
            targetProperty,
            getTargetValue,
            getSourceValue,
            bindingMode,
            UpdateSourceTrigger.Explicit,
            conversionParameters);
        this.sourceProperty.Initialize(this);
    }

    bool IBindingControl.IsUpdatePending
    {
        get => this.dataBindingState.IsUpdatePending;
        set => this.dataBindingState.IsUpdatePending = value;
    }

    /// <summary>
    /// Connects this instance.
    /// </summary>
    /// <param name="currentPhase">The current phase.</param>
    public void Connect(int currentPhase)
    {
        if (this.phase != currentPhase)
        {
            return;
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
        this.dataBindingState.CurrentSource = default;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.Disconnect();
        this.dataBindingState = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TTargetValue IBindingControl<TSourceValue, TTargetValue>.Convert(TSourceValue sourceValue)
    {
        return (TTargetValue)(object)sourceValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IBindingControl.UpdateTargetValue()
    {
        this.UpdateTargetValue();
    }

    private void TryAttachToPropertyChangeNotification()
    {
        this.dataBindingState.CurrentSource = this.sourceProperty.TryUpdateSubscription(BindingMode.OneWay, this.dataBindingState.CurrentSource);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateTargetValue()
    {
        DataBindingHelper.UpdateTargetValue(this, this.dataBindingState);
    }
}