// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetBindingCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class TargetBindingCodeGenerator
{
    private readonly TypeResolver typeResolver;
    private readonly BindingModeResolver bindingModeResolver;
    private readonly TypeAssignmentCompatibilityAssessor typeAssignmentCompatibilityAssessor;
    private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;
    private readonly SourceBindingCodeGenerator sourceBindingCodeGenerator;

    public TargetBindingCodeGenerator(
        TypeResolver typeResolver,
        BindingModeResolver bindingModeResolver,
        TypeAssignmentCompatibilityAssessor typeAssignmentCompatibilityAssessor,
        BindingXamlPlatformInfo bindingXamlPlatformInfo,
        SourceBindingCodeGenerator sourceBindingCodeGenerator)
    {
        this.typeResolver = typeResolver;
        this.bindingModeResolver = bindingModeResolver;
        this.typeAssignmentCompatibilityAssessor = typeAssignmentCompatibilityAssessor;
        this.bindingXamlPlatformInfo = bindingXamlPlatformInfo;
        this.sourceBindingCodeGenerator = sourceBindingCodeGenerator;
    }

    public R<BindingSource> GenerateBinding(
        IDefiniteBinding binding,
        in TargetValueCodeGenerator targetValueCodeGenerator,
        QualifiedType elementType,
        Context context,
        bool hasCodeBehind,
        IAccessorCodeGenerator codeGenerator)
    {
        var elementName = context.XamlElementNameProvider.GetName(binding);
        if (binding.IsBindingToTargetDataContext)
        {
            var bindingSourceResult = codeGenerator.GetBindingSource(false);
            var bindingSource = bindingSourceResult.IsSuccess ? bindingSourceResult.Value : bindingSourceResult.Error;
            context.BindingPathBuilder.AppendLine(
                $@"            var {bindingSource.Name} = {context.BindingSource.Name}.BindTargetDataContextOneWay({binding.Id}, {TargetCodeGenerator.GetTarget(elementType, elementName, hasCodeBehind)}, s => {codeGenerator.GetAccessorGetter()}, ""{codeGenerator.Name}"");");

            context.ExternAliases.TryAdd(elementType);

            return R.Success(bindingSource);
        }

        var qualifiedProperty = codeGenerator.Accessor;
        var bindingMode = this.bindingModeResolver.GetBindingMode(binding.BindingAssignment.Mode, targetValueCodeGenerator, codeGenerator);
        var bindingAssignment = binding.BindingAssignment;
        var updateSourceTriggerType = this.typeResolver.GetType(this.bindingXamlPlatformInfo.UpdateSourceTriggerType);
        var bindingModeType = this.typeResolver.GetType(this.bindingXamlPlatformInfo.BindingModeType);
        var typeAssignmentCompatibility = this.typeAssignmentCompatibilityAssessor.AssessTypeCompatibility(targetValueCodeGenerator.TargetProperty.Type, qualifiedProperty.Type);
        var bindMethodName = this.GetBindMethodName(bindingMode, typeAssignmentCompatibility);
        context.BindingPathBuilder.Append(
            $@"            {context.BindingSource.Name}.{bindMethodName}(
                {binding.Id},
                {TargetCodeGenerator.GetTarget(elementType, elementName, hasCodeBehind)},
                {context.BindingSource.Name}.CreateSourceProperty({this.sourceBindingCodeGenerator.GetSourcePropertyParameters(context, codeGenerator)}),
                s => {codeGenerator.GetAccessorGetter()},
                {targetValueCodeGenerator.GetDependencyProperty()},
                t => {targetValueCodeGenerator.GetPropertyGetter()}");
        if (bindingMode == BindingMode.TwoWay || bindingMode == BindingMode.OneWayToSource || bindingMode == BindingMode.Default)
        {
            context.BindingPathBuilder.Append(@$",
                (s, v) => {codeGenerator.GetAccessorSetter()}");
        }

        context.ExternAliases.TryAdd(bindingModeType);
        context.BindingPathBuilder.Append(@$",
                {bindingModeType.ToAliasQualifiedType()}.{bindingMode}");

        if (!string.IsNullOrEmpty(bindingAssignment.UpdateSourceTrigger))
        {
            context.ExternAliases.TryAdd(updateSourceTriggerType);
            context.BindingPathBuilder.Append(@$",
                updateSourceTrigger: {updateSourceTriggerType.ToAliasQualifiedType()}.{bindingAssignment.UpdateSourceTrigger}");
        }

        context.BindingPathBuilder.AppendLine(");");

        context.ExternAliases.TryAdd(elementType);
        return R.Success(context.BindingSource);
    }

    private string GetBindMethodName(BindingMode bindingMode, TypeAssignmentCompatibility typeAssignmentCompatibility)
    {
        switch (typeAssignmentCompatibility)
        {
            case TypeAssignmentCompatibility.BothWays when bindingMode == BindingMode.TwoWay || bindingMode == BindingMode.Default || bindingMode == BindingMode.OneWayToSource:
                return "BindInvariant";
            case TypeAssignmentCompatibility.TargetToSource when bindingMode == BindingMode.OneWayToSource:
                return "BindInvariant";
            case TypeAssignmentCompatibility.BothWays:
                return "BindInvariantOneWay";
            case TypeAssignmentCompatibility.SourceToTarget when bindingMode == BindingMode.OneTime || bindingMode == BindingMode.OneWay:
                return "BindInvariantOneWay";
            case TypeAssignmentCompatibility.None when bindingMode == BindingMode.OneTime || bindingMode == BindingMode.OneWay:
                return "BindOneWay";
            default:
                return "Bind";
        }
    }
}