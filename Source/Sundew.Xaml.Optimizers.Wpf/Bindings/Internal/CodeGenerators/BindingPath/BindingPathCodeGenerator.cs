// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPathCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Base;
using Sundew.Base.Visiting;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class BindingPathCodeGenerator : IBindingPathVisitor<Parameters, Context, R<BindingSource>, R<CodeInfo>>
{
    private readonly TypeResolver typeResolver;
    private readonly TargetBindingCodeGenerator targetBindingCodeGenerator;
    private readonly TargetBindingPartCodeGenerator targetBindingPartCodeGenerator;
    private TargetValueCodeGenerator targetValue;

    public BindingPathCodeGenerator(
        TypeResolver typeResolver,
        BindingXamlPlatformInfo bindingXamlPlatformInfo,
        ReadOnlyDependencyPropertyToNotificationEventResolver readOnlyDependencyPropertyToNotificationEventResolver,
        BindingModeResolver bindingModeResolver,
        TypeAssignmentCompatibilityAssessor typeAssignmentCompatibilityAssessor)
    {
        this.typeResolver = typeResolver;
        var sourceBindingCodeGenerator = new SourceBindingCodeGenerator(this.typeResolver, readOnlyDependencyPropertyToNotificationEventResolver);
        this.targetBindingCodeGenerator = new TargetBindingCodeGenerator(this.typeResolver, bindingModeResolver, typeAssignmentCompatibilityAssessor, bindingXamlPlatformInfo, sourceBindingCodeGenerator);
        this.targetBindingPartCodeGenerator = new TargetBindingPartCodeGenerator(sourceBindingCodeGenerator, bindingModeResolver, this.typeResolver, bindingXamlPlatformInfo);
    }

    public R<CodeInfo> Visit(IBindingPathExpression bindingPathExpression, Parameters parameters, Context context)
    {
        var binding = parameters.Binding;
        var elementTypeResult = parameters.XamlTypeResolver.Parse(binding.TargetElement.Name);
        if (elementTypeResult.IsError)
        {
            return R.Error();
        }

        var targetValueResult = TargetValueCodeGenerator.GetTarget(elementTypeResult.Value, binding.BindingAssignment, parameters.XamlTypeResolver, this.typeResolver);
        if (!targetValueResult.TryGet(out this.targetValue))
        {
            return R.Error();
        }

        var actualContext = context;
        var visitResult = bindingPathExpression.Visit(this, parameters, context);
        return visitResult.Map(bindingSource => new CodeInfo(actualContext.BindingPathBuilder, bindingSource));
    }

    public void VisitUnknown(IBindingPathExpression bindingPathExpression, Parameters parameters, Context context)
    {
        throw VisitException.Create(bindingPathExpression, parameters, context);
    }

    public R<BindingSource> VisitAttachedDependencyPropertyPart(AttachedDependencyPropertyPart attachedDependencyPropertyPart, Parameters parameters, Context context)
    {
        var codeGenerator = new PropertyAccessorCodeGenerator(context, this.typeResolver, attachedDependencyPropertyPart.Name, true);
        return this.targetBindingPartCodeGenerator.GeneratePartBinding(parameters.Binding.BindingAssignment.Mode, this.targetValue, context, codeGenerator);
    }

    public R<BindingSource> VisitAttachedDependencyProperty(AttachedDependencyProperty attachedDependencyProperty, Parameters parameters, Context context)
    {
        var codeGenerator = new PropertyAccessorCodeGenerator(context, this.typeResolver, attachedDependencyProperty.Name, true);
        return this.targetBindingCodeGenerator.GenerateBinding(parameters.Binding, this.targetValue, this.targetValue.TargetType, context, parameters.HasCodeBehind, codeGenerator);
    }

    public R<BindingSource> VisitIndexerAccessor(IndexerAccessor indexerAccessor, Parameters parameters, Context context)
    {
        var indexerResult = indexerAccessor.Source.Visit(this, parameters, context);
        if (indexerResult.IsError)
        {
            return indexerResult;
        }

        var indexerContext = new Context(indexerResult.Value, context);
        return indexerAccessor.Indexer.Visit(this, parameters, indexerContext);
    }

    public R<BindingSource> VisitIndexerPart(IndexerPart indexerPart, Parameters parameters, Context context)
    {
        var codeGenerator = new IndexerAccessorCodeGenerator(parameters, context, this.typeResolver, indexerPart.Literals);
        return this.targetBindingPartCodeGenerator.GeneratePartBinding(parameters.Binding.BindingAssignment.Mode, this.targetValue, context, codeGenerator);
    }

    public R<BindingSource> VisitIndexer(Indexer indexer, Parameters parameters, Context context)
    {
        var codeGenerator = new IndexerAccessorCodeGenerator(parameters, context, this.typeResolver, indexer.Literals);
        return this.targetBindingCodeGenerator.GenerateBinding(parameters.Binding, this.targetValue, this.targetValue.TargetType, context, parameters.HasCodeBehind, codeGenerator);
    }

    public R<BindingSource> VisitPropertyAccessor(PropertyAccessor propertyAccessor, Parameters parameters, Context context)
    {
        var sourceResult = propertyAccessor.Source.Visit(this, parameters, context);
        if (sourceResult.IsError)
        {
            return sourceResult;
        }

        var propertyContext = new Context(sourceResult.Value, context);
        return propertyAccessor.Property.Visit(this, parameters, propertyContext);
    }

    public R<BindingSource> VisitPropertyPart(PropertyPart propertyPart, Parameters parameters, Context context)
    {
        var codeGenerator = new PropertyAccessorCodeGenerator(context, this.typeResolver, propertyPart.Name, false);
        return this.targetBindingPartCodeGenerator.GeneratePartBinding(parameters.Binding.BindingAssignment.Mode, this.targetValue, context, codeGenerator);
    }

    public R<BindingSource> VisitProperty(Property property, Parameters parameters, Context context)
    {
        var codeGenerator = new PropertyAccessorCodeGenerator(context, this.typeResolver, property.Name, false);
        return this.targetBindingCodeGenerator.GenerateBinding(parameters.Binding, this.targetValue, this.targetValue.TargetType, context, parameters.HasCodeBehind, codeGenerator);
    }

    public R<BindingSource> VisitDataContextSource(DataContextSource dataContextSource, Parameters parameters, Context context)
    {
        var binding = parameters.Binding;
        var elementName = context.XamlElementNameProvider.GetName(binding);
        context.BindingPathBuilder.AppendLine(
            $@"            {context.BindingSource.Name}.BindSourceDataContextOneWay(
                {binding.Id},
                {TargetCodeGenerator.GetTarget(this.targetValue.TargetType, elementName, parameters.HasCodeBehind)},
                {this.targetValue.GetDependencyProperty()},
                t => {this.targetValue.GetPropertyGetter()});");

        context.ExternAliases.TryAdd(this.targetValue.TargetType);
        return R.Success(context.BindingSource);
    }
}