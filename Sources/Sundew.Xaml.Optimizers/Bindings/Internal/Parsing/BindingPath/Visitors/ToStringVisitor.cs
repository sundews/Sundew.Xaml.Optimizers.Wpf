// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToStringVisitor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.Visitors;

using System.Text;
using Sundew.Base;
using Sundew.Base.Visiting;

internal class ToStringVisitor : IBindingPathWalker<__, StringBuilder, string>
{
    public string Visit(IBindingPathExpression bindingPathExpression, __ parameter, StringBuilder stringBuilder = null)
    {
        stringBuilder ??= new StringBuilder();
        bindingPathExpression.Visit(this, parameter, stringBuilder);
        return stringBuilder.ToString();
    }

    public void VisitUnknown(IBindingPathExpression bindingPathExpression, __ parameter, StringBuilder stringBuilder)
    {
        throw VisitException.Create(bindingPathExpression, parameter, stringBuilder);
    }

    public void VisitAttachedDependencyPropertyPart(AttachedDependencyPropertyPart attachedDependencyPropertyPart, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(attachedDependencyPropertyPart);
    }

    public void VisitAttachedDependencyProperty(AttachedDependencyProperty attachedDependencyProperty, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(attachedDependencyProperty);
    }

    public void VisitIndexerAccessor(IndexerAccessor indexerAccessor, __ parameter, in StringBuilder stringBuilder)
    {
        indexerAccessor.Source.Visit(this, parameter, stringBuilder);
        stringBuilder.Append(indexerAccessor.Operator);
        indexerAccessor.Indexer.Visit(this, parameter, stringBuilder);
    }

    public void VisitIndexerPart(IndexerPart indexerPart, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(indexerPart);
    }

    public void VisitIndexer(Indexer indexer, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(indexer);
    }

    public void VisitPropertyAccessor(PropertyAccessor propertyAccessor, __ parameter, in StringBuilder stringBuilder)
    {
        propertyAccessor.Source.Visit(this, parameter,  stringBuilder);
        stringBuilder.Append(propertyAccessor.Operator);
        propertyAccessor.Property.Visit(this, parameter,  stringBuilder);
    }

    public void VisitPropertyPart(PropertyPart propertyPart, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(propertyPart);
    }

    public void VisitProperty(Property property, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(property);
    }

    public void VisitDataContextSource(DataContextSource dataContextSource, __ parameter, in StringBuilder stringBuilder)
    {
        stringBuilder.Append(dataContextSource);
    }
}