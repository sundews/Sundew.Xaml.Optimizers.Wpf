// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingNode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal interface IBindingNode
{
    void Visit<TParameter, TVariable, TResult>(IBindingWalker<TParameter, TVariable, TResult> bindingWalker, TParameter parameter, TVariable variable);

    TInnerResult Visit<TParameter, TVariable, TInnerResult, TResult>(IBindingVisitor<TParameter, TVariable, TInnerResult, TResult> bindingVisitor, TParameter parameter, TVariable variable);
}