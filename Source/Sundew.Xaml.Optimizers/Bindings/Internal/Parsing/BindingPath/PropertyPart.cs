// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyPart.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

internal class PropertyPart : IPropertyExpression
{
    public PropertyPart(string propertyName)
    {
        this.Name = propertyName;
    }

    public string Name { get; }

    public TPartialResult Visit<TParameter, TVariable, TPartialResult, TResult>(IBindingPathVisitor<TParameter, TVariable, TPartialResult, TResult> bindingPathVisitor, TParameter parameter,  TVariable variable)
    {
        return bindingPathVisitor.VisitPropertyPart(this, parameter, variable);
    }

    public void Visit<TParameter, TVariable, TResult>(IBindingPathWalker<TParameter, TVariable, TResult> bindingPathWalker, TParameter parameter, TVariable variable)
    {
        bindingPathWalker.VisitPropertyPart(this, parameter, variable);
    }

    public override string ToString()
    {
        return this.Name;
    }
}