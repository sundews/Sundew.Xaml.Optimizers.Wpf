// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal sealed class Parameters
{
    public Parameters(XamlTypeResolver xamlTypeResolver, IDefiniteBinding binding, bool hasCodeBehind)
    {
        this.XamlTypeResolver = xamlTypeResolver;
        this.Binding = binding;
        this.HasCodeBehind = hasCodeBehind;
    }

    public XamlTypeResolver XamlTypeResolver { get; }

    public IDefiniteBinding Binding { get; }

    public bool HasCodeBehind { get; }
}