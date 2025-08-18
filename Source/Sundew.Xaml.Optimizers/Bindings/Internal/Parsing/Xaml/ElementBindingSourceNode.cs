// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementBindingSourceNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;

internal class ElementBindingSourceNode : IBindingSource
{
    public ElementBindingSourceNode(XElement xElement, string elementName, IReadOnlyList<IBinding> bindings)
    {
        this.TargetElement = xElement;
        this.TargetElementName = elementName;
        this.Bindings = bindings;
    }

    public XElement TargetElement { get; }

    public string TargetElementName { get; }

    public IReadOnlyList<IBinding> Bindings { get; }

    public void Visit<TParameter, TVariable, TResult>(IBindingWalker<TParameter, TVariable, TResult> bindingWalker, TParameter parameter, TVariable variable)
    {
        bindingWalker.ElementBindingSource(this, parameter, variable);
    }

    public TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor, TParameter parameter, TVariable variable)
    {
        return bindingVisitor.ElementBindingSource(this, parameter, variable);
    }

    public override string ToString()
    {
        return $"Source:{this.TargetElement.Name.LocalName}=ByElementName({this.TargetElementName})";
    }
}