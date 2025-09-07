// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Indexer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using System.Collections.Generic;
using System.Text;

internal class Indexer : IIndexerExpression
{
    public Indexer(IReadOnlyList<Literal> literals)
    {
        this.Literals = literals;
    }

    public IReadOnlyList<Literal> Literals { get; }

    public override string ToString()
    {
        return ToString(this.Literals);
    }

    public TPartialResult Visit<TParameter, TVariable, TPartialResult, TResult>(IBindingPathVisitor<TParameter, TVariable, TPartialResult, TResult> bindingPathVisitor, TParameter parameter, TVariable variable)
    {
        return bindingPathVisitor.VisitIndexer(this, parameter, variable);
    }

    public void Visit<TParameter, TVariable, TResult>(IBindingPathWalker<TParameter, TVariable, TResult> bindingPathWalker, TParameter parameter, TVariable variable)
    {
        bindingPathWalker.VisitIndexer(this, parameter, variable);
    }

    internal static string ToString(IReadOnlyList<Literal> literals)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("[");
        foreach (var literal in literals)
        {
            stringBuilder.Append(literal);
            stringBuilder.Append(",");
        }

        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append("]");
        return stringBuilder.ToString();
    }
}