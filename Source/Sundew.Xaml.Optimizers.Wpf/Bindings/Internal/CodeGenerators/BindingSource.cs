// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingSource.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;

using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class BindingSource
{
    public BindingSource(QualifiedType sourceType, string name)
    {
        this.SourceType = sourceType;
        this.Name = name;
    }

    public QualifiedType SourceType { get; }

    public string Name { get; }
}