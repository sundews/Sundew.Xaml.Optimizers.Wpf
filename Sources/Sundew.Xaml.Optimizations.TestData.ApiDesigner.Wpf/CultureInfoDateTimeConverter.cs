namespace Sundew.Xaml.Optimizations.TestData;

using System;
using System.Globalization;
using Sundew.Xaml.Optimizations.Bindings.Converters;

/// <summary>A custom converter.</summary>
/// <seealso cref="ValueConverter{TSource,TTarget}" />
public class CultureInfoDateTimeConverter : ValueConverter<DateTime?, string>
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    public override string Convert(DateTime? source, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        if (parameter is string cultureInfoName)
        {
            cultureInfo = new CultureInfo(cultureInfoName);
        }

        return source?.ToString(cultureInfo);
    }

    public override DateTime? ConvertBack(string target, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        if (parameter is string cultureInfoName)
        {
            cultureInfo = new CultureInfo(cultureInfoName);
        }

        if (string.IsNullOrEmpty(target))
        {
            return default;
        }

        return DateTime.Parse(target, cultureInfo);
    }
}