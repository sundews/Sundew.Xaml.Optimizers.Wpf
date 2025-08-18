// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using Microsoft.CodeAnalysis;

internal sealed class QualifiedType
{
    public const string GlobalAlias = "global";

    public QualifiedType(string assemblyAlias, string assemblyName, string namespaceName, string typeName)
        : this(assemblyAlias, assemblyName, namespaceName, typeName, null)
    {
    }

    public QualifiedType(string assemblyAlias, string assemblyName, string namespaceName, string typeName, ITypeSymbol? typeSymbol)
    {
        this.AssemblyAlias = assemblyAlias;
        this.AssemblyName = assemblyName;
        this.NamespaceName = namespaceName;
        this.TypeName = typeName;
        this.TypeSymbol = typeSymbol;
    }

    public ITypeSymbol? TypeSymbol { get; }

    public bool UsesGlobalAlias => this.AssemblyAlias == GlobalAlias;

    public string AssemblyAlias { get; }

    public string AssemblyName { get; }

    public string NamespaceName { get; }

    public string TypeName { get; }

    public override string ToString()
    {
        return QualifiedSymbolHelper.ToString(this.AssemblyAlias, this.AssemblyName, this.NamespaceName, this.TypeName);
    }

    public string ToNamespaceQualifiedType()
    {
        return QualifiedSymbolHelper.ToNamespaceQualifiedTypeString(this.NamespaceName, this.TypeName);
    }

    public string ToAliasQualifiedType()
    {
        return QualifiedSymbolHelper.ToAliasQualifiedTypeString(this.AssemblyAlias, this.NamespaceName, this.TypeName);
    }
}