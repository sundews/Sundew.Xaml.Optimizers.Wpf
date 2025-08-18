// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceBindingCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;

internal class SourceBindingCodeGenerator
{
    private readonly TypeResolver typeResolver;
    private readonly ReadOnlyDependencyPropertyToNotificationEventResolver readOnlyDependencyPropertyToNotificationEventResolver;

    public SourceBindingCodeGenerator(
        TypeResolver typeResolver,
        ReadOnlyDependencyPropertyToNotificationEventResolver readOnlyDependencyPropertyToNotificationEventResolver)
    {
        this.typeResolver = typeResolver;
        this.readOnlyDependencyPropertyToNotificationEventResolver = readOnlyDependencyPropertyToNotificationEventResolver;
    }

    public string GetSourcePropertyParameters(Context context, IAccessorCodeGenerator codeGenerator)
    {
        var isReadOnlyPropertyBindResult = this.readOnlyDependencyPropertyToNotificationEventResolver.Resolve(
            context.BindingSource.SourceType,
            codeGenerator.Name);

        if (isReadOnlyPropertyBindResult.IsSuccess)
        {
            var delegateType = this.typeResolver.GetType(isReadOnlyPropertyBindResult.Value.NamespaceQualifiedDelegate);
            context.ExternAliases.TryAdd(delegateType);
            return @$"
                    (s, u) =>
                        {{
                            var h = new {delegateType.ToAliasQualifiedType()}((s, e) => u());
                            s.{isReadOnlyPropertyBindResult.Value.EventName} += h;
                            return h;
                        }},
                    (s, h) => s.{isReadOnlyPropertyBindResult.Value.EventName} -= h";
        }

        var fieldInfo = this.typeResolver.TryGetField(context.BindingSource.SourceType, @$"{codeGenerator.Name}Property", true);
        if (fieldInfo.IsSuccess)
        {
            return @$"{context.BindingSource.SourceType.ToAliasQualifiedType()}.{fieldInfo.Value.Name}";
        }

        return @$"""{codeGenerator.Name}""";
    }
}