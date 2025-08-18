// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal readonly struct Parameters
{
    public Parameters(in GeneratorInfo generatorInfo, XamlTypeResolver xamlTypeResolver)
    {
        this.AssemblyName = generatorInfo.AssemblyName;
        this.Namespace = generatorInfo.Namespace;
        this.XamlTypeResolver = xamlTypeResolver;
    }

    public string AssemblyName { get; }

    public string Namespace { get; }

    public XamlTypeResolver XamlTypeResolver { get; }
}