// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataBindingState{TRoot,TSource,TSourceValue,TTarget,TTargetValue}.cs" company="Sundews">
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

internal interface IDataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue>
    where TTarget : DependencyObject
{
    IBindingContext<TRoot, TSource> BindingContext { get; }

    TTarget Target { get; }

    DependencyProperty TargetProperty { get; }

    Func<TTarget, TTargetValue> GetTargetValue { get; }

    BindingMode BindingMode { get; }

    UpdateSourceTrigger UpdateSourceTrigger { get; }

    ConversionParameters<TSourceValue, TTargetValue> ConversionParameters { get; }

    TSource CurrentSource { get; set; }

    bool IsUpdatePending { get; set; }

    bool IsUpdating { get; set; }

    bool TryGetSourceValue(out TSourceValue sourceValue);
}