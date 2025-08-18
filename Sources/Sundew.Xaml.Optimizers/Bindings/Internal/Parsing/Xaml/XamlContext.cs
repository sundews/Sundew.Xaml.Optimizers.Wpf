// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class XamlContext
{
    public XamlContext(string namespaceName, string baseTypeName, XamlTypeResolver xamlTypeResolver, BindingMarkupExtensionParser bindingMarkupExtensionParser, XamlElementNameResolver xamlElementNameResolver)
    {
        this.NamespaceName = namespaceName;
        this.BaseTypeName = baseTypeName;
        this.XamlTypeResolver = xamlTypeResolver;
        this.BindingMarkupExtensionParser = bindingMarkupExtensionParser;
        this.XamlElementNameResolver = xamlElementNameResolver;
    }

    public string NamespaceName { get; }

    public string BaseTypeName { get; }

    public XamlTypeResolver XamlTypeResolver { get; }

    public BindingMarkupExtensionParser BindingMarkupExtensionParser { get; }

    public XamlElementNameResolver XamlElementNameResolver { get; }

    public List<BindingRootNode> BindingRootNodes { get; } = new List<BindingRootNode>();
}