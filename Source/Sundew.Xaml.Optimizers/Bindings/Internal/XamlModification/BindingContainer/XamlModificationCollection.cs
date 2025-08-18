// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlModificationCollection.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using System.Collections.Generic;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class XamlModificationCollection
{
    public XamlModificationCollection(IReadOnlyList<XamlModificationInfo> xamlModificationInfos, IReadOnlyDictionary<BindingRootNode, QualifiedType> bindingRootNodeTypes)
    {
        this.XamlModificationInfos = xamlModificationInfos;
        this.BindingRootNodeTypes = bindingRootNodeTypes;
    }

    public IReadOnlyList<XamlModificationInfo> XamlModificationInfos { get; }

    public IReadOnlyDictionary<BindingRootNode, QualifiedType> BindingRootNodeTypes { get; }
}