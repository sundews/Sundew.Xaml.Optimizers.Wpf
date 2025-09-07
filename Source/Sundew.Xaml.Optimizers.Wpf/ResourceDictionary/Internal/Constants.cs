// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary.Internal;

using System.Xml.Linq;

internal static class Constants
{
    public const string SourceText = "Source";
    public const string DefaultResourceDictionaryXPath = "//default:ResourceDictionary";
    public const string DefaultResourceDictionaryMergedDictionariesXPath = "//default:ResourceDictionary/default:ResourceDictionary.MergedDictionaries";
    public const string DefaultResourceDictionaryMergedDictionariesDefaultResourceDictionaryXPath = "//default:ResourceDictionary/default:ResourceDictionary.MergedDictionaries/default:ResourceDictionary";
    public const string SxPrefix = "sx";
    public const string ResourceDictionaryName = "ResourceDictionary";
    public const string ThemeResourceDictionaryName = "ThemeResourceDictionary";
    public const string ThemeModeResourceDictionaryName = "ThemeModeResourceDictionary";
    public const string OptimizationCategoryName = "ResourceDictionary.Category";
    public const string MergedDictionaries = ".MergedDictionaries";
    public static readonly XNamespace SundewXamlOptimizationNamespace = XNamespace.Get("http://sundew.dev/xaml");
    public static readonly XNamespace SundewXamlThemingNamespace = XNamespace.Get("http://sundew.dev/xaml");
    public static readonly XName SundewXamlResourceDictionaryName = SundewXamlOptimizationNamespace + Constants.ResourceDictionaryName;
    public static readonly XName SundewXamlOptimizationCategoryName = SundewXamlOptimizationNamespace + Constants.OptimizationCategoryName;
}