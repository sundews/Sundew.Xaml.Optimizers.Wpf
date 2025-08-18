// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConversionParameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using Sundew.Xaml.Optimizations.Bindings.Converters;

internal readonly struct ConversionParameters<TSourceValue, TTargetValue>
{
    public ConversionParameters(IValueConverter<TSourceValue, TTargetValue> valueConverter, object converterParameter, Lazy<TTargetValue> fallbackValue, Lazy<TTargetValue> targetNullValue)
    {
        this.ValueConverter = valueConverter;
        this.ConverterParameter = converterParameter;
        this.FallbackValue = fallbackValue;
        this.TargetNullValue = targetNullValue;
    }

    public IValueConverter<TSourceValue, TTargetValue> ValueConverter { get; }

    public object ConverterParameter { get; }

    public Lazy<TTargetValue> FallbackValue { get; }

    public Lazy<TTargetValue> TargetNullValue { get; }
}