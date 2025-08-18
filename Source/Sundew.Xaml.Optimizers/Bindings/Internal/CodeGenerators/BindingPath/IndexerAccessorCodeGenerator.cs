// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexerAccessorCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

internal class IndexerAccessorCodeGenerator : IAccessorCodeGenerator
{
    private readonly Context context;
    private readonly TypeResolver typeResolver;
    private readonly IEnumerable<QualifiedType> parameterTypes;
    private readonly string constants;

    public IndexerAccessorCodeGenerator(Parameters parameters, Context context, TypeResolver typeResolver, IReadOnlyList<Literal> literals)
    {
        this.context = context;
        this.typeResolver = typeResolver;
        this.parameterTypes = GetParameterTypes(literals, parameters);
        this.constants = GetConstants(literals);
    }

    public string Name => "Item[]";

    public QualifiedProperty Accessor => this.typeResolver.GetIndexer(this.context.BindingSource.SourceType, this.parameterTypes);

    public R<BindingSource, BindingSource> GetBindingSource(bool acceptsSharedSource)
    {
        return this.context.BindingSourceProvider.GetOrAddIndexer(this.context.BindingSource, this.parameterTypes);
    }

    public string GetAccessorGetter() => $"s[{this.constants}]";

    public string GetAccessorSetter() => $"s[{this.constants}] = v";

    private static IEnumerable<QualifiedType> GetParameterTypes(IReadOnlyList<Literal> literals, Parameters parameters)
    {
        return literals.Select(x => parameters.XamlTypeResolver.GetQualifiedType(x.Type));
    }

    private static string GetConstants(IReadOnlyList<Literal> literals)
    {
        var stringBuilder = new StringBuilder();
        for (int i = 1; i < literals.Count; i++)
        {
            stringBuilder.Append(", ");
            var literal = literals[i];
            GetConstant(literal, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private static void GetConstant(Literal literal, StringBuilder stringBuilder)
    {
        if (literal.Type != null)
        {
            stringBuilder.Append(literal.Value);
            return;
        }

        stringBuilder.Append(@"""");
        stringBuilder.Append(literal.Value);
        stringBuilder.Append(@"""");
    }
}