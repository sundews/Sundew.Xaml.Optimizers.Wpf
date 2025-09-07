// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedField.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using Microsoft.CodeAnalysis;

internal sealed class QualifiedField
{
    public QualifiedField(QualifiedType type, string name, IFieldSymbol fieldSymbol)
    {
        this.Type = type;
        this.FieldSymbol = fieldSymbol;
        this.Name = name;
    }

    public QualifiedType Type { get; }

    public string Name { get; }

    public bool IsStatic => this.FieldSymbol.IsStatic;

    internal IFieldSymbol FieldSymbol { get; }
}