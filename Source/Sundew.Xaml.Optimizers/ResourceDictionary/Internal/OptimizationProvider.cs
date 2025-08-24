// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Provides an optimization info based on an <see cref="XElement"/>.
/// </summary>
internal sealed class OptimizationProvider
{
    /// <summary>
    /// Gets the optimization information.
    /// </summary>
    /// <param name="resourceDictionaryElement">The resource dictionary element.</param>
    /// <param name="defaultReplacementType">The default replacement type.</param>
    /// <param name="replaceUncategorized">The replace uncategorized.</param>
    /// <param name="optimizationMappings">The optimization mappings.</param>
    /// <param name="frameworkResourceDictionaryName">Name of the framework resource dictionary.</param>
    /// <returns>
    /// The optimization info.
    /// </returns>
    public static OptimizationInfo GetOptimizationInfo(
        XElement resourceDictionaryElement,
        XamlType defaultReplacementType,
        bool replaceUncategorized,
        IReadOnlyList<OptimizationMapping> optimizationMappings,
        XName frameworkResourceDictionaryName)
    {
        if (resourceDictionaryElement.Name == frameworkResourceDictionaryName)
        {
            var sourceAttribute = resourceDictionaryElement.Attribute(Constants.SourceText);
            if (sourceAttribute == null)
            {
                return new OptimizationInfo(OptimizationAction.None, defaultReplacementType, string.Empty);
            }

            var categoryAttribute = resourceDictionaryElement.Attributes().FirstOrDefault(x => x.Name.LocalName.EndsWith(Constants.SundewXamlOptimizationCategoryName.LocalName));
            if (categoryAttribute == null)
            {
                if (replaceUncategorized)
                {
                    return new OptimizationInfo(OptimizationAction.Replace, defaultReplacementType, sourceAttribute.Value);
                }

                return new OptimizationInfo(OptimizationAction.None, defaultReplacementType, sourceAttribute.Value);
            }

            var optimizationMapping = optimizationMappings.FirstOrDefault(x => x.Category == categoryAttribute.Value);
            if (optimizationMapping != null)
            {
                categoryAttribute.Remove();
                return optimizationMapping.Action switch
                {
                    OptimizationAction.None => new OptimizationInfo(OptimizationAction.None, defaultReplacementType, string.Empty),
                    OptimizationAction.Remove => new OptimizationInfo(OptimizationAction.Remove, defaultReplacementType, sourceAttribute.Value),
                    OptimizationAction.Replace => new OptimizationInfo(OptimizationAction.Replace, XamlType.TryParse(optimizationMapping.XamlType) ?? defaultReplacementType, sourceAttribute.Value),
                    _ => new OptimizationInfo(OptimizationAction.None, defaultReplacementType, string.Empty),
                };
            }

            return new OptimizationInfo(OptimizationAction.None, defaultReplacementType, sourceAttribute.Value);
        }

        return new OptimizationInfo(OptimizationAction.None, defaultReplacementType, string.Empty);
    }
}