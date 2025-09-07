// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAccessorCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class PropertyAccessorCodeGenerator : IAccessorCodeGenerator
{
    private readonly Context context;
    private readonly TypeResolver typeResolver;
    private readonly bool isAttachedDependencyProperty;

    public PropertyAccessorCodeGenerator(in Context context, in TypeResolver typeResolver, string propertyName, bool isAttachedDependencyProperty)
    {
        this.context = context;
        this.typeResolver = typeResolver;
        this.isAttachedDependencyProperty = isAttachedDependencyProperty;
        this.Accessor = this.typeResolver.GetProperty(this.context.BindingSource.SourceType, propertyName);
    }

    public string Name => this.Accessor.Name;

    public QualifiedProperty Accessor { get; }

    public R<BindingSource, BindingSource> GetBindingSource(bool acceptsSharedSource)
    {
        return this.context.BindingSourceProvider.GetOrAddProperty(this.context.BindingSource, this.Accessor.Name, acceptsSharedSource);
    }

    public string GetAccessorGetter()
    {
        if (this.isAttachedDependencyProperty)
        {
            return $"{this.context.BindingSource.SourceType.ToAliasQualifiedType()}.Get{this.Accessor.Name}(s)";
        }

        return $"s.{this.Accessor.Name}";
    }

    public string GetAccessorSetter()
    {
        if (this.isAttachedDependencyProperty)
        {
            return $"{this.context.BindingSource.SourceType.ToAliasQualifiedType()}.Set{this.Accessor.Name}(s, v)";
        }

        return $"s.{this.Accessor.Name} = v";
    }
}