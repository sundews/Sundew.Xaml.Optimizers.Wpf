// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextTargetBindingNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal class DataContextTargetBindingNode : IBindingSource, IDefiniteBinding
{
    public DataContextTargetBindingNode(
        XElement targetElement,
        string elementName,
        BindingAssignment bindingAssignment,
        int id,
        IReadOnlyList<IBinding> bindings,
        bool isEnabled)
    {
        this.TargetElement = targetElement;
        this.TargetElementName = elementName;
        this.BindingAssignment = bindingAssignment;
        this.Id = id;
        this.Bindings = bindings;
        this.IsEnabled = isEnabled;
    }

    public XElement TargetElement { get; }

    public string TargetElementName { get; }

    public BindingAssignment BindingAssignment { get; }

    public int Id { get; }

    public bool IsEnabled { get; }

    public bool IsBindingToTargetDataContext => true;

    public IReadOnlyList<IBinding> Bindings { get; }

    public void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.DataContextTargetBinding(this, parameter, variable);
    }

    public TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.DataContextTargetBinding(this, parameter, variable);
    }

    public override string ToString()
    {
        return $"Source:{this.TargetElement.Name.LocalName}{(string.IsNullOrEmpty(this.TargetElementName) ? string.Empty : $"({this.TargetElementName})")}->{this.BindingAssignment}";
    }
}