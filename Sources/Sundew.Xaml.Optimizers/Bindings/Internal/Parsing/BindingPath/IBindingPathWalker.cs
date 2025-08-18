// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingPathWalker.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Sundew.Base.Visiting;

internal interface IBindingPathWalker<in TParameter, TVariable, out TResult> : IVisitor<IBindingPathExpression, TParameter, TVariable, TResult>
{
    void VisitAttachedDependencyPropertyPart(AttachedDependencyPropertyPart attachedDependencyPropertyPart, TParameter parameter, in TVariable variable);

    void VisitAttachedDependencyProperty(AttachedDependencyProperty attachedDependencyProperty, TParameter parameter, in TVariable variable);

    void VisitIndexerAccessor(IndexerAccessor indexerAccessor, TParameter parameter, in TVariable variable);

    void VisitIndexerPart(IndexerPart indexerPart, TParameter parameter, in TVariable variable);

    void VisitIndexer(Indexer indexer, TParameter parameter, in TVariable variable);

    void VisitPropertyAccessor(PropertyAccessor propertyAccessor, TParameter parameter, in TVariable variable);

    void VisitPropertyPart(PropertyPart propertyPart, TParameter parameter, in TVariable context);

    void VisitProperty(Property property, TParameter parameter, in TVariable context);

    void VisitDataContextSource(DataContextSource dataContextSource, TParameter parameter, in TVariable variable);
}