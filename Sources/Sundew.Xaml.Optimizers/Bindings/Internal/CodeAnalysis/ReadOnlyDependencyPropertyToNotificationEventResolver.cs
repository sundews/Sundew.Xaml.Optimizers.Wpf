// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDependencyPropertyToNotificationEventResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System;
using System.Collections.Generic;
using Sundew.Base;

internal class ReadOnlyDependencyPropertyToNotificationEventResolver
{
    private readonly CodeAnalyzer codeAnalyzer;
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>> readOnlyDependencyPropertyToNotificationEvents;

    public ReadOnlyDependencyPropertyToNotificationEventResolver(CodeAnalyzer codeAnalyzer, Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>> readOnlyDependencyPropertyToNotificationEvents)
    {
        this.codeAnalyzer = codeAnalyzer;
        this.readOnlyDependencyPropertyToNotificationEvents = readOnlyDependencyPropertyToNotificationEvents;
    }

    public R<ReadOnlyDependencyPropertyToNotificationEvent> Resolve(QualifiedType qualifiedType, string propertyName)
    {
        var typeSymbol = this.codeAnalyzer.GetTypeSymbol(qualifiedType);
        while (typeSymbol != null)
        {
            if (this.readOnlyDependencyPropertyToNotificationEvents.Value.TryGetValue($"{typeSymbol.ContainingAssembly.Name}|{typeSymbol.ToDisplayString()}", out var assemblyType))
            {
                if (assemblyType.TryGetValue(propertyName, out var value))
                {
                    return R.Success(value);
                }
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return R.Error();
    }
}