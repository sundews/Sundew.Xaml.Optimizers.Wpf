// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingRootNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;

internal class BindingRootNode : IBindingNode
{
    public BindingRootNode(
        XElement xElement,
        string namespaceQualifiedType,
        string name,
        List<IBinding> bindings,
        bool hasCodeBehind)
    {
        this.XElement = xElement;
        this.NamespaceQualifiedType = namespaceQualifiedType;
        this.Name = name;
        this.Bindings = bindings;
        this.HasCodeBehind = hasCodeBehind;
    }

    public XElement XElement { get; }

    public string NamespaceQualifiedType { get; }

    public string Name { get; }

    public IReadOnlyList<IBinding> Bindings { get; }

    public bool HasCodeBehind { get; }

    public override string ToString()
    {
        return $"BindingRootNode<{(string.IsNullOrEmpty(this.Name) ? this.XElement.Name.ToString() : this.Name)}>";
    }

    public void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.BindingRoot(this, parameter, variable);
    }

    public TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.BindingRoot(this, parameter, variable);
    }
}