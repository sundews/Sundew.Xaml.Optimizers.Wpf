// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.ResourceDictionary.Internal;

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
    /// <param name="frameworkResourceDictionaryName">Name of the framework resource dictionary.</param>
    /// <returns>
    /// The optimization info.
    /// </returns>
    public static OptimizationInfo GetOptimizationInfo(
        XElement resourceDictionaryElement,
        XName frameworkResourceDictionaryName)
    {
        if (resourceDictionaryElement.Name == frameworkResourceDictionaryName)
        {
            var sourceAttribute = resourceDictionaryElement.Attribute(Constants.SourceText);
            if (sourceAttribute == null)
            {
                return new OptimizationInfo(OptimizationMode.TVoid, null);
            }

            var match = Constants.UriRegex.Match(sourceAttribute.Value);
            if (!match.Success)
            {
                return new OptimizationInfo(OptimizationMode.TVoid, null);
            }

            var unsharedWpfGroup = match.Groups[Constants.UnsharedWpfText];
            if (unsharedWpfGroup.Success)
            {
                return new OptimizationInfo(OptimizationMode.TVoid, null);
            }

            return new OptimizationInfo(OptimizationMode.Shared, match.Value);
        }

        return new OptimizationInfo(OptimizationMode.TVoid, null);
    }
}