// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CastingValueConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>A value converter that casts.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <seealso cref="IValueConverter{TSourceValue,TTargetValue}" />
public class CastingValueConverter<TSource, TTarget> : IValueConverter<TSource, TTarget>
{
    private readonly IValueConverter valueConverter;

    /// <summary>Initializes a new instance of the <see cref="CastingValueConverter{TSource, TTarget}"/> class.</summary>
    /// <param name="valueConverter">The value converter.</param>
    public CastingValueConverter(IValueConverter valueConverter)
    {
        this.valueConverter = valueConverter;
    }

    /// <summary>Converts the specified source.</summary>
    /// <param name="source">The source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>The target value.</returns>
    public TTarget Convert(TSource source, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        return (TTarget)this.valueConverter.Convert(source, targetType, parameter, cultureInfo);
    }

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <span class="keyword"><span class="languageSpecificText"><span class="cs">null</span><span class="vb">Nothing</span><span class="cpp">nullptr</span></span></span><span class="nu">a null reference (<span class="keyword">Nothing</span> in Visual Basic)</span>, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return this.valueConverter.Convert(value, targetType, parameter, culture);
    }

    /// <summary>Converts the back.</summary>
    /// <param name="target">The target.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>The source value.</returns>
    public TSource ConvertBack(TTarget target, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        return (TSource)this.valueConverter.ConvertBack(target, targetType, parameter, cultureInfo);
    }

    /// <summary>Converts a value.</summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <span class="keyword"><span class="languageSpecificText"><span class="cs">null</span><span class="vb">Nothing</span><span class="cpp">nullptr</span></span></span><span class="nu">a null reference (<span class="keyword">Nothing</span> in Visual Basic)</span>, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return this.valueConverter.ConvertBack(value, targetType, parameter, culture);
    }
}