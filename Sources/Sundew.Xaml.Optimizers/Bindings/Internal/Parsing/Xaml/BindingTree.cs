// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingTree.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;

internal class BindingTree : IBindingNode
{
    public BindingTree(List<BindingRootNode> bindingRoots)
    {
        this.BindingRoots = bindingRoots;
    }

    public List<BindingRootNode> BindingRoots { get; }

    public void Visit<TParameter, TVariable, TResult>(
        IBindingWalker<TParameter, TVariable, TResult> bindingWalker,
        TParameter parameter,
        TVariable variable)
    {
        bindingWalker.BindingTree(this, parameter, variable);
    }

    public TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(
        IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor,
        TParameter parameter,
        TVariable variable)
    {
        return bindingVisitor.BindingTree(this, parameter, variable);
    }
}