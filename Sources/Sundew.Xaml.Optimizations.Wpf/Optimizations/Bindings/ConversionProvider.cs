// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConversionProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif
using Sundew.Xaml.Optimizations.Bindings.Converters;

internal static class ConversionProvider
{
    internal static readonly Dictionary<Type, Type[]> SimpleConversionTypes = new Dictionary<Type, Type[]>
    {
        { typeof(DateTime), new[] { typeof(string) } },
        { typeof(sbyte), new[] { typeof(string), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(byte), new[] { typeof(string), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(short), new[] { typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(ushort), new[] { typeof(string), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(int), new[] { typeof(string), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(uint), new[] { typeof(string), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(long), new[] { typeof(string), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(char), new[] { typeof(string), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
        { typeof(float), new[] { typeof(string), typeof(double) } },
        { typeof(ulong), new[] { typeof(string), typeof(float), typeof(double), typeof(decimal) } },
    };

    public static ConversionParameters<TSourceValue, TTargetValue> GetConversionParameters<TSourceValue, TTargetValue>(DependencyObject target, int id)
    {
        GetBindingData<TSourceValue, TTargetValue>(target, id, out var bindingData, out var valueConverter);
        return new ConversionParameters<TSourceValue, TTargetValue>(valueConverter, bindingData?.ConverterParameter, GetConverter<TTargetValue>(bindingData?.FallbackValue), GetConverter<TTargetValue>(bindingData?.TargetNullValue));
    }

    public static ConversionParameters<TSourceValue, TTargetValue> GetConversionParameters<TSourceValue, TTargetValue>(DependencyObject target, int id, BindingMode bindingMode)
    {
        GetBindingData<TSourceValue, TTargetValue>(target, id, out var bindingData, out var valueConverter);
        if (valueConverter == null)
        {
            valueConverter = TypeValueConverter<TSourceValue, TTargetValue>.Instance;
            if (valueConverter == null)
            {
                switch (bindingMode)
                {
                    case BindingMode.TwoWay:
                        if (typeof(TSourceValue) != typeof(TTargetValue))
                        {
                            throw new NotSupportedException();
                        }

                        break;
                    case BindingMode.OneWay:
                    case BindingMode.OneTime:
                        if (!typeof(TTargetValue).IsAssignableFrom(typeof(TSourceValue)))
                        {
                            throw new NotSupportedException();
                        }

                        break;
#if WPF
                    case BindingMode.OneWayToSource:
                        if (!typeof(TSourceValue).IsAssignableFrom(typeof(TTargetValue)))
                        {
                            throw new NotSupportedException();
                        }

                        break;
#endif
                }
            }
        }

        return new ConversionParameters<TSourceValue, TTargetValue>(valueConverter, bindingData?.ConverterParameter, GetConverter<TTargetValue>(bindingData?.FallbackValue), GetConverter<TTargetValue>(bindingData?.TargetNullValue));
    }

    public static TypeValueConverter<TSourceValue, TTargetValue> GetConverter<TSourceValue, TTargetValue>(Type sourceType)
    {
        Func<TSourceValue, Type, object, CultureInfo, TTargetValue> convert = null;
        Func<TTargetValue, Type, object, CultureInfo, TSourceValue> convertBack = null;
        if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            sourceType = sourceType.GenericTypeArguments.First();
        }

        if (SimpleConversionTypes.TryGetValue(sourceType, out var types) && types.Contains(typeof(TTargetValue)))
        {
            convert = (value, type, _, _) => (TTargetValue)Convert.ChangeType(value, type);
            convertBack = (value, type, _, _) => (TSourceValue)Convert.ChangeType(value, type);
            return new TypeValueConverter<TSourceValue, TTargetValue>(convert, convertBack);
        }

        var sourceConverter = TypeDescriptor.GetConverter(sourceType);
        if (sourceConverter.CanConvertTo(typeof(TTargetValue)))
        {
            convert = (value, type, _, language) => (TTargetValue)sourceConverter.ConvertTo(null, language, value, type);
        }

        if (sourceConverter.CanConvertFrom(typeof(TTargetValue)))
        {
            convertBack = (value, _, _, language) => (TSourceValue)sourceConverter.ConvertFrom(null, language, value);
        }

        TypeConverter targetConverter = null;
        if (convert == null)
        {
            targetConverter = TypeDescriptor.GetConverter(typeof(TTargetValue));
            if (targetConverter.CanConvertFrom(sourceType))
            {
                convert = (value, _, _, cultureInfo) => (TTargetValue)targetConverter.ConvertFrom(null, cultureInfo, value);
            }
        }

        if (convertBack == null)
        {
            targetConverter ??= TypeDescriptor.GetConverter(typeof(TTargetValue));
            if (targetConverter.CanConvertTo(sourceType))
            {
                convertBack = (value, type, _, language) => (TSourceValue)targetConverter.ConvertTo(null, language, value, type);
            }
        }

        return convert != null && convertBack != null ? new TypeValueConverter<TSourceValue, TTargetValue>(convert, convertBack) : null;
    }

    private static void GetBindingData<TSourceValue, TTargetValue>(DependencyObject target, int id, out BindingData bindingData, out IValueConverter<TSourceValue, TTargetValue> valueConverter)
    {
        valueConverter = null;
        bindingData = null;
        var bindingMetadata = BindingConnection.GetMetadata(target);
        if (bindingMetadata?.TryGetById(id, out bindingData) == true)
        {
            var converter = bindingData.Converter;
            if (converter != null)
            {
                if (converter is IValueConverter<TSourceValue, TTargetValue> newValueConverter)
                {
                    valueConverter = newValueConverter;
                }
                else
                {
                    valueConverter = new CastingValueConverter<TSourceValue, TTargetValue>(converter);
                }
            }
        }
    }

    private static Lazy<TTargetValue> GetConverter<TTargetValue>(object value)
    {
        if (value != null)
        {
            return new Lazy<TTargetValue>(
                () =>
                {
                    IValueConverter<object, TTargetValue> converter = GetConverter<object, TTargetValue>(value.GetType());
                    if (converter != null)
                    {
                        return converter.Convert(value, typeof(TTargetValue), null, null);
                    }

                    return default;
                });
        }

        return new Lazy<TTargetValue>(() => default);
    }
}