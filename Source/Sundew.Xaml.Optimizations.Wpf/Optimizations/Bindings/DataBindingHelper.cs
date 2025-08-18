// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBindingHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Sundew.Xaml.Optimizations.Bindings.Internals;

#if WINDOWS_UWP
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
#endif
using Sundew.Xaml.Optimizations.Bindings.Converters;

internal static class DataBindingHelper
{
    private static readonly ThreadLocal<Engine> Engine = new ThreadLocal<Engine>();

#if WINDOWS_UWP
        public static Engine EnsureInitialized(CoreDispatcher dispatcher)
#else
    public static Engine EnsureInitialized(Dispatcher dispatcher)
#endif
    {
        if (!Engine.IsValueCreated)
        {
            Engine.Value = new Engine(new BindingDispatcher(dispatcher));
        }

        return Engine.Value;
    }

    public static void TryRegisterLostFocus<TTarget>(TTarget target, UpdateSourceTrigger updateSourceTrigger, RoutedEventHandler lostFocusEventHandler)
        where TTarget : DependencyObject
    {
        if (updateSourceTrigger == UpdateSourceTrigger.LostFocus && target is UIElement newUiElement)
        {
            newUiElement.LostFocus += lostFocusEventHandler;
        }
    }

    public static void GetNotificationParameters(DependencyObject target, DependencyProperty targetProperty, ref BindingMode bindingMode, ref UpdateSourceTrigger updateSourceTrigger)
    {
#if WPF
        if (target != null && targetProperty != null)
        {
            var metadata = targetProperty.GetMetadata(target);
            if (metadata is FrameworkPropertyMetadata frameworkPropertyMetadata)
            {
                if (bindingMode == BindingMode.Default)
                {
                    bindingMode = frameworkPropertyMetadata.BindsTwoWayByDefault ? BindingMode.TwoWay : BindingMode.OneWay;
                }

                if (updateSourceTrigger == UpdateSourceTrigger.Default)
                {
                    updateSourceTrigger = frameworkPropertyMetadata.DefaultUpdateSourceTrigger;
                }
            }
        }
#endif

        if (updateSourceTrigger == UpdateSourceTrigger.Default)
        {
            updateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool RequiresSourcePropertyChangeNotification(BindingMode bindingMode)
    {
        return bindingMode == BindingMode.OneWay || bindingMode == BindingMode.TwoWay;
    }

    public static void Refresh<TSourceValue, TTargetValue>(
        BindingMode bindingMode,
        IBindingControl<TSourceValue, TTargetValue> bindingControl)
    {
        switch (bindingMode)
        {
#if WPF
            case BindingMode.OneWayToSource:
                break;
#endif
            default:
                bindingControl.UpdateTargetValue();
                break;
        }
    }

    public static void Refresh<TSourceValue, TTargetValue>(
        BindingMode bindingMode,
        ITwoWayBindingControl<TSourceValue, TTargetValue> bindingControl)
    {
        switch (bindingMode)
        {
#if WPF
            case BindingMode.OneWayToSource:
                bindingControl.UpdateSourceValue();
                break;
#endif
            default:
                bindingControl.UpdateTargetValue();
                break;
        }
    }

    public static void UpdateSourceValue<TRoot, TSource, TSourceValue, TTarget, TTargetValue>(
        ITwoWayBindingControl<TSourceValue, TTargetValue> bindingControl,
        ref DataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue> dataBindingState,
        Action<TSource, TSourceValue> setSourceValue)
        where TTarget : DependencyObject
    {
        if (dataBindingState.IsUpdating)
        {
            return;
        }

        var source = dataBindingState.BindingContext.Source;
        if (source != null)
        {
            var target = dataBindingState.Target;
            if (target != null)
            {
                var targetValue = dataBindingState.GetTargetValue(target);
                var sourceValue = dataBindingState.ConversionParameters.ValueConverter != null
                    ? dataBindingState.ConversionParameters.ValueConverter.ConvertBack(
                        targetValue,
                        typeof(TSourceValue),
                        dataBindingState.ConversionParameters.ConverterParameter,
#if WINDOWS_UWP
                            (string)target.GetValue(FrameworkElement.LanguageProperty))
#else
                        ((XmlLanguage)target.GetValue(FrameworkElement.LanguageProperty)).GetSpecificCulture())
#endif
                    : bindingControl.GetSourceValue(targetValue);
                /*if (!Equals(sourceValue, dataBindingState.CurrentSourceValue))
                {*/
                dataBindingState.IsUpdating = true;
                //// dataBindingState.CurrentSourceValue = sourceValue;
                setSourceValue(source, sourceValue);
                dataBindingState.IsUpdating = false;
                //// }
            }
        }

        dataBindingState.CurrentSource = source;
    }

    public static void UpdateTargetValue<TRoot, TSource, TSourceValue, TTarget, TTargetValue>(
        IBindingControl<TSourceValue, TTargetValue> bindingControl,
        IDataBindingState<TRoot, TSource, TSourceValue, TTarget, TTargetValue> dataBindingState)
        where TTarget : DependencyObject
    {
        if (dataBindingState.IsUpdating)
        {
            return;
        }

        TTargetValue targetValue = default;
        var setValue = false;
        try
        {
            var target = dataBindingState.Target;
            if (target != null)
            {
                setValue = true;
                if (dataBindingState.TryGetSourceValue(out var sourceValue))
                {
                    targetValue = dataBindingState.ConversionParameters.ValueConverter != null ? dataBindingState.ConversionParameters.ValueConverter.Convert(
                            sourceValue,
                            typeof(TTargetValue),
                            dataBindingState.ConversionParameters.ConverterParameter,
#if WINDOWS_UWP
                                (string)target.GetValue(FrameworkElement.LanguageProperty))
#else
                            ((XmlLanguage)target.GetValue(FrameworkElement.LanguageProperty)).GetSpecificCulture())
#endif
                        : bindingControl.Convert(sourceValue);
                }
                else
                {
                    targetValue = dataBindingState.ConversionParameters.TargetNullValue.Value;
                }
            }
        }
        catch (Exception)
        {
            setValue = true;
            targetValue = dataBindingState.ConversionParameters.FallbackValue.Value;
        }
        finally
        {
            /*if (!Equals(targetValue, dataBindingState.CurrentTargetValue))
             {*/
            if (setValue)
            {
                dataBindingState.IsUpdating = true;
                //// dataBindingState.CurrentTargetValue = targetValue;
#if WINDOWS_UWP
                    dataBindingState.Target.SetValue(dataBindingState.TargetProperty, targetValue);
#else
                dataBindingState.Target.SetCurrentValue(dataBindingState.TargetProperty, targetValue);
#endif
                dataBindingState.IsUpdating = false;
                //// }
            }
        }
    }
}