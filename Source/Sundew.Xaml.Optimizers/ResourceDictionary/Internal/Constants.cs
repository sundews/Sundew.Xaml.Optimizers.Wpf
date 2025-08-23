// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

using System.Xml.Linq;

internal static class Constants
{
    public const string SourceText = "Source";
    public const string DefaultResourceDictionaryMergedDictionariesDefaultResourceDictionaryXPath = "//default:ResourceDictionary/default:ResourceDictionary.MergedDictionaries/default:ResourceDictionary";
    public const string SxPrefix = "sx";
    public const string ResourceDictionaryName = "ResourceDictionary";
    public const string OptimizationCategoryName = "ResourceDictionary.Category";
    public static readonly XNamespace SundewXamlOptimizationWpfNamespace = XNamespace.Get("http://sundew.dev/xaml");
    public static readonly XName SundewXamlResourceDictionaryName = SundewXamlOptimizationWpfNamespace + Constants.ResourceDictionaryName;
    public static readonly XName SundewXamlOptimizationCategoryName = SundewXamlOptimizationWpfNamespace + Constants.OptimizationCategoryName;
}