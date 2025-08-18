// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBinding.cs" company="Sundews">
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
using System.Windows.Controls;
using System.Windows.Data;
#endif

/// <summary>A one way binding to a DataContext.</summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <typeparam name="TTargetValue">The type of the target value.</typeparam>
/// <seealso cref="Sundew.Xaml.Optimizations.Bindings.IBinding" />
public class DataContextBinding<TRoot, TSource, TTarget, TTargetValue> : IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>, IBinding, IBindingControl<TSource, TTargetValue>
    where TTarget : DependencyObject
{
    private ConversionParameters<TSource, TTargetValue> conversionParameters;
    private IBindingContext<TRoot, TSource> bindingContext;
    private TTarget target;
    private DependencyProperty targetProperty;
    private Func<TTarget, TTargetValue> getTargetValue;
    private bool isUpdatePending;
    private TSource currentSource;

    /// <summary>Initializes a new instance of the <see cref="DataContextBinding{TRoot, TSource, TTarget, TTargetValue}"/> class.</summary>
    /// <param name="id">The identifier.</param>
    /// <param name="bindingContext">The binding context.</param>
    /// <param name="target">The target.</param>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="getTargetValue">The get target value.</param>
    public DataContextBinding(
        in int id,
        IBindingContext<TRoot, TSource> bindingContext,
        TTarget target,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue)
    {
        this.conversionParameters = ConversionProvider.GetConversionParameters<TSource, TTargetValue>(target, id, BindingMode.OneWay);
        this.bindingContext = bindingContext;
        this.target = target;
        this.targetProperty = targetProperty;
        this.getTargetValue = getTargetValue;
    }

    bool IBindingControl.IsUpdatePending
    {
        get => this.isUpdatePending;
        set => this.isUpdatePending = value;
    }

    IBindingContext<TRoot, TSource> IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.BindingContext => this.bindingContext;

    TTarget IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.Target => this.target;

    DependencyProperty IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.TargetProperty => this.targetProperty;

    Func<TTarget, TTargetValue> IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.GetTargetValue => this.getTargetValue;

    BindingMode IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.BindingMode => BindingMode.OneWay;

    UpdateSourceTrigger IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.UpdateSourceTrigger => UpdateSourceTrigger.Explicit;

    ConversionParameters<TSource, TTargetValue> IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.ConversionParameters => this.conversionParameters;

    TSource IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.CurrentSource { get => this.currentSource; set => this.currentSource = value; }

    bool IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.IsUpdatePending { get => this.isUpdatePending; set => this.isUpdatePending = value; }

    bool IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.IsUpdating { get; set; }

    /// <summary>
    /// Connects this instance.
    /// </summary>
    public void Connect(int currentPhase)
    {
        if (currentPhase != 0)
        {
            return;
        }

        this.Refresh();
    }

    /// <summary>Refreshes this instance.</summary>
    public void Refresh()
    {
        DataBindingHelper.UpdateTargetValue(this, this);
    }

    /// <summary>
    /// Disconnects this instance.
    /// </summary>
    public void Disconnect()
    {
        this.currentSource = default;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        this.Disconnect();
        this.conversionParameters = default;
        this.bindingContext = default;
        this.target = default;
        this.targetProperty = default;
        this.getTargetValue = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TTargetValue IBindingControl<TSource, TTargetValue>.Convert(TSource sourceValue)
    {
        return (TTargetValue)(object)sourceValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IBindingControl.UpdateTargetValue()
    {
        this.Refresh();
    }

    bool IDataBindingState<TRoot, TSource, TSource, TTarget, TTargetValue>.TryGetSourceValue(out TSource source)
    {
        source = this.bindingContext.Source;
        return true;
    }
}