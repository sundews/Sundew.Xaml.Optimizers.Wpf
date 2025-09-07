// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using Sundew.Base.Visiting;

internal interface IBindingVisitor<in TParameter, in TVariable, out TInnerResult, out TResult> : IVisitor<IBindingNode, TParameter, TVariable, TResult>
{
    TInnerResult BindingTree(BindingTree bindingTree, TParameter parameter, TVariable variable);

    TInnerResult BindingRoot(BindingRootNode bindingRootNode, TParameter parameter, TVariable variable);

    TInnerResult CastDataContextBindingSource(CastDataContextBindingSourceNode castSourceBinding, TParameter parameter, TVariable variable);

    TInnerResult DataContextTargetBinding(DataContextTargetBindingNode dataContextTargetBindingNode, TParameter parameter, TVariable variable);

    TInnerResult DataTemplateCastDataContextBindingSource(DataTemplateCastDataContextBindingSourceNode dataTemplateCastDataContextBindingSourceNode, TParameter parameter, TVariable variable);

    TInnerResult ControlTemplateCastDataContextBindingSource(ControlTemplateCastDataContextBindingSourceNode controlTemplateCastDataContextBindingSourceNode, TParameter parameter, TVariable variable);

    TInnerResult ElementBindingSource(ElementBindingSourceNode elementBindingSourceNode, TParameter parameter, TVariable variable);

    TInnerResult Binding(BindingNode bindingNode, TParameter parameter, TVariable variable);
}