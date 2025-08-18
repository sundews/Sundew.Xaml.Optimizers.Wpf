// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CastDataContextBindingSourceNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class CastDataContextBindingSourceNode : IBindingSource
{
    public CastDataContextBindingSourceNode(XElement xElement, string elementName, string className, QualifiedType castType, IReadOnlyList<IBinding> bindings)
    {
        this.TargetElement = xElement;
        this.TargetElementName = elementName;
        this.ClassName = className;
        this.CastType = castType;
        this.Bindings = bindings;
    }

    public XElement TargetElement { get; }

    public string TargetElementName { get; }

    public string ClassName { get; }

    public string Name => string.IsNullOrEmpty(this.ClassName) ? this.TargetElement.Name.ToString() : this.ClassName;

    public QualifiedType CastType { get; }

    public IReadOnlyList<IBinding> Bindings { get; }

    public virtual void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.CastDataContextSourceBinding(this, parameter, variable);
    }

    public virtual TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.CastDataContextBindingSource(this, parameter, variable);
    }

    public override string ToString()
    {
        var lastDotIndex = this.Name.LastIndexOf('.');
        return $"Source:{(lastDotIndex > -1 ? this.Name.Substring(lastDotIndex + 1) : this.Name)}{(string.IsNullOrEmpty(this.TargetElementName) ? string.Empty : $"({this.TargetElementName})")}->DataContext<{this.CastType}>={this.Name}.DataContext";
    }
}