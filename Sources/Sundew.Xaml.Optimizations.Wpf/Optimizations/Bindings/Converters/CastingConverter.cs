// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CastingConverter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Converters;

using System;
using System.Globalization;

/// <summary>A value converter that casts.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <seealso cref="IValueConverter{TSourceValue,TTargetValue}" />
public class CastingConverter<TSource, TTarget> : ValueConverter<TSource, TTarget>
{
    /// <summary>Converts the specified source.</summary>
    /// <param name="source">The source.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>The target value.</returns>
    public override TTarget Convert(TSource source, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        object value = source;
        return (TTarget)value;
    }

    /// <summary>Converts the back.</summary>
    /// <param name="target">The target.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="cultureInfo">The culture information.</param>
    /// <returns>The source value.</returns>
    public override TSource ConvertBack(TTarget target, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        object value = target;
        return (TSource)value;
    }

    /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.</summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The object value to set on the property where the extension is applied.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}