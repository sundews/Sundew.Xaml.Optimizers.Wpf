// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Xaml.Optimization;

internal class CodeAnalyzer
{
    private readonly Lazy<Compilation> compilation;

    public CodeAnalyzer(
        string assemblyName,
        IReadOnlyList<IFileReference> compiles,
        IReadOnlyList<IAssemblyReference> assemblyReferences,
        IFile file,
        XamlTypeBaseTypeSourceCodeGenerator xamlTypeBaseTypeSourceCodeGenerator)
    {
        this.compilation = new Lazy<Compilation>(() =>
        {
            var syntaxTrees = compiles.Select(x => CSharpSyntaxTree.ParseText(
                file.ReadAllText(x.Path)));

            return CSharpCompilation.Create(
                assemblyName,
                syntaxTrees.ToArray(),
                assemblyReferences.Select(
                        x => MetadataReference.CreateFromFile(
                            x.Path,
                            new MetadataReferenceProperties(aliases: x.Aliases.ToImmutableArray())))
                    .ToArray());
        });
    }

    public Compilation Compilation => this.compilation.Value;

    public ITypeSymbol? GetTypeSymbol(QualifiedType qualifiedType)
    {
        return qualifiedType.TypeSymbol ?? this.Compilation.GetTypeByMetadataName(qualifiedType.ToNamespaceQualifiedType());
    }

    public TMemberType? TryGetMember<TMemberType>(QualifiedType qualifiedType, string memberName, Func<ImmutableArray<ISymbol>, TMemberType> selectorFunction)
    {
        var typeSymbol = this.GetTypeSymbol(qualifiedType);
        while (typeSymbol != null)
        {
            var member = selectorFunction(typeSymbol.GetMembers(memberName));
            if (member != null)
            {
                return member;
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return default;
    }

    public IReadOnlyList<string>? GetAssemblyAliases(ISymbol symbol)
    {
        var assemblySymbols = this.Compilation.References.Select(x =>
        {
            var assemblyOrModuleSymbol = this.Compilation.GetAssemblyOrModuleSymbol(x);
            var assemblySymbol = assemblyOrModuleSymbol as IAssemblySymbol;
            if (assemblySymbol == null && assemblyOrModuleSymbol is IModuleSymbol moduleSymbol)
            {
                assemblySymbol = moduleSymbol.ContainingAssembly;
            }

            return (assemblySymbol, x.Properties.Aliases);
        });
        return assemblySymbols.FirstOrDefault(x => x.assemblySymbol != null && EqualityComparer<IAssemblySymbol>.Default.Equals(x.assemblySymbol, symbol.ContainingAssembly)).Aliases;
    }
}