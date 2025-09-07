// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetBindingPartCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class TargetBindingPartCodeGenerator
{
    private readonly SourceBindingCodeGenerator sourceBindingCodeGenerator;
    private readonly BindingModeResolver bindingModeResolver;
    private readonly TypeResolver typeResolver;
    private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;

    public TargetBindingPartCodeGenerator(SourceBindingCodeGenerator sourceBindingCodeGenerator, BindingModeResolver bindingModeResolver, TypeResolver typeResolver, BindingXamlPlatformInfo bindingXamlPlatformInfo)
    {
        this.sourceBindingCodeGenerator = sourceBindingCodeGenerator;
        this.bindingModeResolver = bindingModeResolver;
        this.typeResolver = typeResolver;
        this.bindingXamlPlatformInfo = bindingXamlPlatformInfo;
    }

    public R<BindingSource> GeneratePartBinding(BindingMode suggestedBindingMode, in TargetValueCodeGenerator targetValueCodeGenerator, Context context, IAccessorCodeGenerator codeGenerator)
    {
        var bindingSourceResult = codeGenerator.GetBindingSource(true);
        if (bindingSourceResult.IsSuccess)
        {
            var bindingModeType = this.typeResolver.GetType(this.bindingXamlPlatformInfo.BindingModeType);
            var bindingMode = this.bindingModeResolver.GetBindingMode(suggestedBindingMode, targetValueCodeGenerator, codeGenerator);
            context.BindingPathBuilder.AppendLine(
                $@"            var {bindingSourceResult.Value.Name} = {context.BindingSource.Name}.BindPart(
                {context.BindingSource.Name}.CreateSourceProperty({this.sourceBindingCodeGenerator.GetSourcePropertyParameters(context, codeGenerator)}),
                s => {codeGenerator.GetAccessorGetter()},
                {bindingModeType.ToAliasQualifiedType()}.{bindingMode});");

            context.ExternAliases.TryAdd(bindingModeType);
        }

        return R.Success(bindingSourceResult.IsSuccess ? bindingSourceResult.Value : bindingSourceResult.Error);
    }
}