// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingContainerInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;

using System.Collections.Generic;
using System.Text;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class BindingContainerInfo
{
    public BindingContainerInfo(
        QualifiedType bindingContainerType,
        string namespaceQualifiedBindingRootType,
        StringBuilder codeBuilder,
        HashSet<string> externalAliases)
    {
        this.BindingContainerType = bindingContainerType;
        this.NamespaceQualifiedBindingRootType = namespaceQualifiedBindingRootType;
        this.CodeBuilder = codeBuilder;
        this.ExternalAliases = externalAliases;
    }

    public QualifiedType BindingContainerType { get; }

    public string NamespaceQualifiedBindingRootType { get; }

    public StringBuilder CodeBuilder { get; }

    public HashSet<string> ExternalAliases { get; }
}