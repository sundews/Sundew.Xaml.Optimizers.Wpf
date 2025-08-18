// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTemplateCastDataContextBindingSourceNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class DataTemplateCastDataContextBindingSourceNode : CastDataContextBindingSourceNode
{
    public DataTemplateCastDataContextBindingSourceNode(XElement xElement, string elementName, string dataTemplateKey, XElement contentElement, QualifiedType castType, IReadOnlyList<IBinding> bindings)
        : base(xElement, elementName, null, castType, bindings)
    {
        this.DataTemplateKey = dataTemplateKey;
        this.ContentElement = contentElement;
    }

    public string DataTemplateKey { get; }

    public XElement ContentElement { get; }

    public static DataTemplateCastDataContextBindingSourceNode Create(
        XElement xElement,
        string elementName,
        string dataTemplateKey,
        XElement contentElement,
        QualifiedType castType,
        IReadOnlyList<IBinding> bindings)
    {
        return new DataTemplateCastDataContextBindingSourceNode(xElement, elementName, dataTemplateKey, contentElement, castType, bindings);
    }

    public override void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.DataTemplateCastDataContextBindingSource(this, parameter, variable);
    }

    public override TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.DataTemplateCastDataContextBindingSource(this, parameter, variable);
    }

    public override string ToString()
    {
        return $"Source:{this.ContentElement.Name.LocalName}{(string.IsNullOrEmpty(this.TargetElementName) ? string.Empty : $"({this.TargetElementName})")}->DataContext<{this.CastType}>={this.ContentElement.Name}.DataContext";
    }
}