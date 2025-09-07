// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingCompilerSettings.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System.Collections.Generic;

/// <summary>Contains settings for the binding compiler.</summary>
public class BindingCompilerSettings
{
    /// <summary>Initializes a new instance of the <see cref="BindingCompilerSettings"/> class.</summary>
    /// <param name="readOnlyDependencyPropertyToNotificationEvents">The read only dependency property bindings.</param>
    /// <param name="xamlTypeToSourceCodeTypes">The xaml type to source code types.</param>
    /// <param name="oneWayBindingProperties">The one way binding properties.</param>
    /// <param name="optInToOptimizations">A value indicating whether xaml files must opt in to optimizations.</param>
    public BindingCompilerSettings(
        Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>> readOnlyDependencyPropertyToNotificationEvents,
        Dictionary<string, IReadOnlyDictionary<string, Namespace>> xamlTypeToSourceCodeTypes,
        Dictionary<string, IReadOnlyCollection<string>> oneWayBindingProperties,
        bool optInToOptimizations = false)
    {
        this.ReadOnlyDependencyPropertyToNotificationEvents = readOnlyDependencyPropertyToNotificationEvents;
        this.XamlTypeToSourceCodeTypes = xamlTypeToSourceCodeTypes;
        this.OneWayBindingProperties = oneWayBindingProperties;
        this.OptInToOptimizations = optInToOptimizations;
    }

    /// <summary>Gets the read only dependency property bindings.</summary>
    /// <value>The read only dependency property bindings.</value>
    public Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>> ReadOnlyDependencyPropertyToNotificationEvents { get; }

    /// <summary>Gets the type of the xaml type to source code.</summary>
    /// <value>The type of the xaml type to source code.</value>
    public Dictionary<string, IReadOnlyDictionary<string, Namespace>> XamlTypeToSourceCodeTypes { get; }

    /// <summary>Gets the one way binding properties.</summary>
    /// <value>The one way binding properties.</value>
    public Dictionary<string, IReadOnlyCollection<string>> OneWayBindingProperties { get; }

    /// <summary>Gets a value indicating whether [automatic optimize].</summary>
    /// <value>
    ///   <c>true</c> if [automatic optimize]; otherwise, <c>false</c>.</value>
    public bool OptInToOptimizations { get; }
}