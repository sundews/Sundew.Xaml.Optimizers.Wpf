// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizationInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary.Internal;

using Sundew.Xaml.Optimization;

/// <summary>
/// Info about how a resource dictionary should be optimized.
/// </summary>
internal sealed record OptimizationInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizationInfo"/> class.
    /// </summary>
    /// <param name="optimizationAction">The optimization action.</param>
    /// <param name="replacementType">The replacement type.</param>
    /// <param name="source">The binding.</param>
    /// <param name="xamlDiagnostic">The xaml diagnostic.</param>
    public OptimizationInfo(OptimizationAction optimizationAction, XamlType replacementType, string source, XamlDiagnostic? xamlDiagnostic = null)
    {
        this.OptimizationAction = optimizationAction;
        this.ReplacementType = replacementType;
        this.Source = source;
        this.XamlDiagnostic = xamlDiagnostic;
    }

    /// <summary>
    /// Gets the optimization mode.
    /// </summary>
    /// <value>
    /// The optimization mode.
    /// </value>
    public OptimizationAction OptimizationAction { get; }

    /// <summary>
    /// Gets the replacement type.
    /// </summary>
    public XamlType ReplacementType { get; }

    /// <summary>
    /// Gets the binding.
    /// </summary>
    /// <value>
    /// The binding.
    /// </value>
    public string Source { get; }

    /// <summary>
    /// Gets diagnostic information related to XAML processing, if available.
    /// </summary>
    public XamlDiagnostic? XamlDiagnostic { get; }
}