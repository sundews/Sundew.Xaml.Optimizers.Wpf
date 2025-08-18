// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBindingState.cs" company="Sundews">
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
using Sundew.Xaml.Optimizations.Bindings.Internals;

internal sealed class DataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue> : IDataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue>, IDataBindingState
    where TTarget : DependencyObject
{
    private Func<TSource, TSourceValue> getSourceValue;

    public DataBindingState(
        IBindingContext<TRoot, TSource> bindingContext,
        TTarget target,
        DependencyProperty targetProperty,
        Func<TTarget, TTargetValue> getTargetValue,
        Func<TSource, TSourceValue> getSourceValue,
        BindingMode bindingMode,
        UpdateSourceTrigger updateSourceTrigger,
        in ConversionParameters<TSourceValue, TTargetValue> conversionParameters)
    {
        this.BindingContext = bindingContext;
        this.Target = target;
        this.TargetProperty = targetProperty;
        this.GetTargetValue = getTargetValue;
        this.getSourceValue = getSourceValue;
        this.BindingMode = bindingMode;
        this.UpdateSourceTrigger = updateSourceTrigger;
        this.ConversionParameters = conversionParameters;
        this.CurrentSource = default;
        this.IsUpdating = false;
    }

    public IBindingContext<TRoot, TSource> BindingContext { get; }

    public TTarget Target { get; }

    DependencyObject IDataBindingState.Target => this.Target;

    public DependencyProperty TargetProperty { get; }

    public Func<TTarget, TTargetValue> GetTargetValue { get; }

    public BindingMode BindingMode { get; }

    public UpdateSourceTrigger UpdateSourceTrigger { get; }

    public ConversionParameters<TSourceValue, TTargetValue> ConversionParameters { get; }

    public TSource CurrentSource { get; set; }

    public bool IsUpdatePending { get; set; }

    public bool IsUpdating { get; set; }

    bool IDataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue>.TryGetSourceValue(out TSourceValue sourceValue)
    {
        var source = this.BindingContext.Source;
        if (source != null)
        {
            sourceValue = this.getSourceValue(source);
            return true;
        }

        sourceValue = default;
        return false;
    }
}