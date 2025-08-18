// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameters.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer;

using System.Collections.Generic;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal sealed class Parameters
{
    public Parameters(
        in GeneratorInfo generatorInfo,
        XamlTypeResolver xamlTypeResolver,
        IReadOnlyDictionary<BindingRootNode, QualifiedType> bindingRootNodeTypes)
    {
        this.OutputPath = generatorInfo.OutputPath;
        this.AssemblyName = generatorInfo.AssemblyName;
        this.Namespace = generatorInfo.Namespace;
        this.XamlTypeResolver = xamlTypeResolver;
        this.BindingRootNodeTypes = bindingRootNodeTypes;
    }

    public string OutputPath { get; }

    public string AssemblyName { get; }

    public string Namespace { get; }

    public XamlTypeResolver XamlTypeResolver { get; }

    public IReadOnlyDictionary<BindingRootNode, QualifiedType> BindingRootNodeTypes { get; }
}