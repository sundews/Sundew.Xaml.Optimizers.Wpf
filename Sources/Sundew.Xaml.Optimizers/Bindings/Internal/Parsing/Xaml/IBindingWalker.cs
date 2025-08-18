// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingWalker.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using Sundew.Base.Visiting;

internal interface IBindingWalker<in TParameter, in TVariable, out TResult> : IVisitor<IBindingNode, TParameter, TVariable, TResult>
{
    void BindingTree(BindingTree bindingTree, TParameter parameter, TVariable variable);

    void BindingRoot(BindingRootNode bindingRootNode, TParameter parameter, TVariable variable);

    void CastDataContextSourceBinding(CastDataContextBindingSourceNode castSourceBinding, TParameter parameter, TVariable variable);

    void DataTemplateCastDataContextBindingSource(DataTemplateCastDataContextBindingSourceNode dataTemplateCastDataContextBindingSourceNode, TParameter parameter, TVariable variable);

    void ControlTemplateCastDataContextBindingSource(ControlTemplateCastDataContextBindingSourceNode controlTemplateCastDataContextBindingSourceNode, TParameter parameter, TVariable variable);

    void DataContextTargetBinding(DataContextTargetBindingNode dataContextTargetBindingNode, TParameter parameter, TVariable variable);

    void ElementBindingSource(ElementBindingSourceNode elementBindingSourceNode, TParameter parameter, TVariable variable);

    void Binding(BindingNode bindingNode, TParameter parameter, TVariable variable);
}