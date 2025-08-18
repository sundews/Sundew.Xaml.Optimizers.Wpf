// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAccessor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Newtonsoft.Json;

internal class PropertyAccessor : IBindingPathExpression
{
    private const string Dot = ".";

    public PropertyAccessor(IBindingPathExpression source, IPropertyExpression property)
    {
        this.Source = source;
        this.Property = property;
    }

    public IBindingPathExpression Source { get; }

    [JsonIgnore]
    public string Operator => Dot;

    public IPropertyExpression Property { get; }

    public TPartialResult Visit<TParameter, TVariable, TPartialResult, TResult>(IBindingPathVisitor<TParameter, TVariable, TPartialResult, TResult> bindingPathVisitor, TParameter parameter, TVariable variable)
    {
        return bindingPathVisitor.VisitPropertyAccessor(this, parameter, variable);
    }

    public void Visit<TParameter, TVariable, TResult>(IBindingPathWalker<TParameter, TVariable, TResult> bindingPathWalker, TParameter parameter, TVariable variable)
    {
        bindingPathWalker.VisitPropertyAccessor(this, parameter, variable);
    }

    public override string ToString()
    {
        return $"{this.Source}{this.Operator}{this.Property}";
    }
}