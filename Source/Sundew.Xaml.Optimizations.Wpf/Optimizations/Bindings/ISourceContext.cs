// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISourceContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using Sundew.Xaml.Optimizations.Bindings.Internals;

/// <summary>Interface for implementing a source provider.</summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
public interface ISourceContext<out TSource>
{
    /// <summary>Gets the source.</summary>
    /// <value>The source.</value>
    TSource Source { get; }

    /// <summary>Gets the engine.</summary>
    /// <value>The engine.</value>
    Engine Engine { get; }
}