// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedSymbolFactory.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System.Linq;
using Microsoft.CodeAnalysis;

internal class QualifiedSymbolFactory
{
    public static QualifiedType CreateType(CodeAnalyzer codeAnalyzer, ITypeSymbol typeSymbol)
    {
        return new QualifiedType(
            codeAnalyzer.GetAssemblyAliases(typeSymbol)?.FirstOrDefault() ?? QualifiedType.GlobalAlias,
            typeSymbol.ContainingAssembly.ToString(),
            typeSymbol.ContainingNamespace.ToString(),
            typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            typeSymbol);
    }

    public static QualifiedProperty CreateProperty(CodeAnalyzer codeAnalyzer, IPropertySymbol propertySymbol, string propertyName)
    {
        return new QualifiedProperty(
            CreateType(codeAnalyzer, propertySymbol.Type),
            propertyName,
            propertySymbol.GetMethod != null && propertySymbol.GetMethod.DeclaredAccessibility == Accessibility.Public,
            propertySymbol.SetMethod != null && propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Public,
            propertySymbol);
    }

    public static QualifiedField CreateField(CodeAnalyzer codeAnalyzer, string fieldName, IFieldSymbol fieldSymbol)
    {
        return new QualifiedField(CreateType(codeAnalyzer, fieldSymbol.Type), fieldName, fieldSymbol);
    }
}