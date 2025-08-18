// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexerAccessor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Newtonsoft.Json;

internal class IndexerAccessor : IBindingPathExpression
{
    public IndexerAccessor(IBindingPathExpression source, IIndexerExpression indexer)
    {
        this.Source = source;
        this.Indexer = indexer;
    }

    public IBindingPathExpression Source { get; }

    [JsonIgnore]
    public string Operator => string.Empty;

    public IIndexerExpression Indexer { get; }

    public TPartialResult Visit<TParameter, TVariable, TPartialResult, TResult>(IBindingPathVisitor<TParameter, TVariable, TPartialResult, TResult> bindingPathVisitor, TParameter parameter, TVariable variable)
    {
        return bindingPathVisitor.VisitIndexerAccessor(this, parameter, variable);
    }

    public void Visit<TParameter, TVariable, TResult>(IBindingPathWalker<TParameter, TVariable, TResult> bindingPathWalker, TParameter parameter, TVariable variable)
    {
        bindingPathWalker.VisitIndexerAccessor(this, parameter, variable);
    }

    public override string ToString()
    {
        return $"{this.Source}{this.Operator}{this.Indexer}";
    }
}