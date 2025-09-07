// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using Microsoft.CodeAnalysis;

internal sealed class QualifiedProperty
{
    public QualifiedProperty(QualifiedType type, string name, bool hasGetter, bool hasSetter)
        : this(type, name, hasGetter, hasSetter, null)
    {
    }

    public QualifiedProperty(QualifiedType type, string name, bool hasGetter, bool hasSetter, IPropertySymbol propertySymbol)
    {
        this.Type = type;
        this.Name = name;
        this.HasGetter = hasGetter;
        this.HasSetter = hasSetter;
        this.PropertySymbol = propertySymbol;
    }

    public QualifiedType Type { get; }

    public string Name { get; }

    public bool HasGetter { get; }

    public bool HasSetter { get; }

    internal IPropertySymbol PropertySymbol { get; }
}