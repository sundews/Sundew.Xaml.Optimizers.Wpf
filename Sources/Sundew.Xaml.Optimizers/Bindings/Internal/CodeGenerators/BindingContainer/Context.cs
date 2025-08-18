// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer;

using System.Collections.Generic;
using System.Text;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal sealed class Context
{
    public Context(
        XamlElementNameProvider xamlElementNameProvider,
        BindingSourceProvider bindingSourceProvider,
        BindingSource? bindingSource = null,
        List<BindingContainerInfo>? containerInfos = null,
        StringBuilder? bindingContainerSourceCodeBuilder = null,
        HashSet<string>? externAliases = null,
        bool hasCodeBehind = false)
    {
        this.XamlElementNameProvider = xamlElementNameProvider;
        this.BindingSourceProvider = bindingSourceProvider;
        this.ExternAliases = externAliases ?? new HashSet<string>();
        this.BindingContainerSourceCodeBuilder = bindingContainerSourceCodeBuilder ?? new StringBuilder();
        this.BindingContainerInfos = containerInfos ?? new List<BindingContainerInfo>();
        this.BindingSource = bindingSource;
        this.HasCodeBehind = hasCodeBehind;
    }

    public Context(StringBuilder bindingContainerSourceCodeBuilder, BindingSource bindingSource, Context context)
        : this(context.XamlElementNameProvider, context.BindingSourceProvider, bindingSource, context.BindingContainerInfos, bindingContainerSourceCodeBuilder, context.ExternAliases, context.HasCodeBehind)
    {
    }

    public Context(
        StringBuilder bindingContainerSourceCodeBuilder,
        HashSet<string> externalAliases,
        Context context,
        bool hasCodeBehind)
        : this(context.XamlElementNameProvider, context.BindingSourceProvider, context.BindingSource, context.BindingContainerInfos, bindingContainerSourceCodeBuilder, externalAliases, hasCodeBehind)
    {
    }

    public HashSet<string> ExternAliases { get; }

    public bool HasCodeBehind { get; }

    public List<BindingContainerInfo> BindingContainerInfos { get; }

    public XamlElementNameProvider XamlElementNameProvider { get; }

    public BindingSourceProvider BindingSourceProvider { get; }

    public StringBuilder BindingContainerSourceCodeBuilder { get; }

    public BindingSource? BindingSource { get; }
}