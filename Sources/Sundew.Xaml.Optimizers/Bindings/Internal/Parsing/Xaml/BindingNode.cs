// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal class BindingNode : IDefiniteBinding
{
    public BindingNode(XElement xElement, string elementName, BindingAssignment bindingAssignment, int id, bool isEnabled)
    {
        this.TargetElement = xElement;
        this.TargetElementName = elementName;
        this.BindingAssignment = bindingAssignment;
        this.Id = id;
        this.IsEnabled = isEnabled;
    }

    public XElement TargetElement { get; }

    public string TargetElementName { get; }

    public BindingAssignment BindingAssignment { get; }

    public int Id { get; }

    public bool IsEnabled { get; }

    public bool IsBindingToTargetDataContext => false;

    public void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.Binding(this, parameter, variable);
    }

    public TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.Binding(this, parameter, variable);
    }

    public override string ToString()
    {
        var bindingMarkupExtension = this.BindingAssignment.ToString();
        return
            $"{this.TargetElement.Name.LocalName}{(string.IsNullOrEmpty(this.TargetElementName) ? string.Empty : $"({this.TargetElementName})")}->{bindingMarkupExtension}{(bindingMarkupExtension.EndsWith("=") ? $"{this.TargetElement.Name.LocalName}.DataContext" : string.Empty)}";
    }
}