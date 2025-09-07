// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using System.Collections.Generic;
using System.Text;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal sealed class Context
{
    public Context(
        StringBuilder bindingPathBuilder,
        BindingSource bindingSource,
        XamlElementNameProvider xamlElementNameProvider,
        BindingSourceProvider bindingSourceProvider,
        HashSet<string> externAliases)
    {
        this.XamlElementNameProvider = xamlElementNameProvider;
        this.BindingSourceProvider = bindingSourceProvider;
        this.ExternAliases = externAliases;
        this.BindingPathBuilder = bindingPathBuilder;
        this.BindingSource = bindingSource;
    }

    public Context(StringBuilder bindingPathBuilder, BindingSource bindingSource, Context context)
        : this(bindingPathBuilder, bindingSource, context.XamlElementNameProvider, context.BindingSourceProvider, context.ExternAliases)
    {
    }

    public Context(BindingSource bindingSource, Context context)
        : this(context.BindingPathBuilder, bindingSource, context)
    {
        this.BindingSource = bindingSource;
    }

    public XamlElementNameProvider XamlElementNameProvider { get; }

    public BindingSourceProvider BindingSourceProvider { get; }

    public HashSet<string> ExternAliases { get; }

    public StringBuilder BindingPathBuilder { get; }

    public BindingSource BindingSource { get; }
}