// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingPathVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Sundew.Base.Visiting;

internal interface IBindingPathVisitor<in TParameter, in TVariable, out TInnerResult, out TResult> : IVisitor<IBindingPathExpression, TParameter, TVariable, TResult>
{
    TInnerResult VisitAttachedDependencyPropertyPart(AttachedDependencyPropertyPart attachedDependencyPropertyPart, TParameter parameter, TVariable variable);

    TInnerResult VisitAttachedDependencyProperty(AttachedDependencyProperty attachedDependencyProperty, TParameter parameter, TVariable variable);

    TInnerResult VisitIndexerAccessor(IndexerAccessor indexerAccessor, TParameter parameter, TVariable variable);

    TInnerResult VisitIndexerPart(IndexerPart indexerPart, TParameter parameter, TVariable variable);

    TInnerResult VisitIndexer(Indexer indexer, TParameter parameter, TVariable variable);

    TInnerResult VisitPropertyAccessor(PropertyAccessor propertyAccessor, TParameter parameter, TVariable variable);

    TInnerResult VisitPropertyPart(PropertyPart propertyPart, TParameter parameter, TVariable context);

    TInnerResult VisitProperty(Property property, TParameter parameter, TVariable context);

    TInnerResult VisitDataContextSource(DataContextSource dataContextSource, TParameter parameter, TVariable variable);
}