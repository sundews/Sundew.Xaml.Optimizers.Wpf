// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexerPart.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using System.Collections.Generic;

internal class IndexerPart : IIndexerExpression
{
    public IndexerPart(IReadOnlyList<Literal> literals)
    {
        this.Literals = literals;
    }

    public IReadOnlyList<Literal> Literals { get; }

    public override string ToString()
    {
        return Indexer.ToString(this.Literals);
    }

    public TPartialResult Visit<TParameter, TVariable, TPartialResult, TResult>(IBindingPathVisitor<TParameter, TVariable, TPartialResult, TResult> bindingPathVisitor, TParameter parameter, TVariable variable)
    {
        return bindingPathVisitor.VisitIndexerPart(this, parameter, variable);
    }

    public void Visit<TParameter, TVariable, TResult>(IBindingPathWalker<TParameter, TVariable, TResult> bindingPathWalker, TParameter parameter, TVariable variable)
    {
        bindingPathWalker.VisitIndexerPart(this, parameter, variable);
    }
}