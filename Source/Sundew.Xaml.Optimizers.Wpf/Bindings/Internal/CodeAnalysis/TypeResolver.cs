// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.Exceptions;

internal class TypeResolver
{
    private const string IndexerName = "this[]";
    private readonly CodeAnalyzer codeAnalyzer;
    private readonly Dictionary<string, QualifiedType> namespaceQualifiedTypeCache = new Dictionary<string, QualifiedType>();

    public TypeResolver(CodeAnalyzer codeAnalyzer)
    {
        this.codeAnalyzer = codeAnalyzer;
    }

    public QualifiedType GetType(string namespaceQualifiedName)
    {
        if (this.namespaceQualifiedTypeCache.TryGetValue(namespaceQualifiedName, out var qualifiedType))
        {
            return qualifiedType;
        }

        var typeSymbol = this.codeAnalyzer.Compilation.GetTypeByMetadataName(namespaceQualifiedName);
        if (typeSymbol == null)
        {
            throw new TypeNotFoundException($"The namespace qualified type: {namespaceQualifiedName} could not be found");
        }

        qualifiedType = QualifiedSymbolFactory.CreateType(this.codeAnalyzer, typeSymbol);
        this.namespaceQualifiedTypeCache.Add(namespaceQualifiedName, qualifiedType);
        return qualifiedType;
    }

    public QualifiedProperty GetProperty(QualifiedType qualifiedType, string propertyName)
    {
        var result = this.TryGetProperty(qualifiedType, propertyName);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new MemberNotFoundException(qualifiedType.ToAliasQualifiedType(), propertyName);
    }

    public R<QualifiedProperty> TryGetProperty(QualifiedType qualifiedType, string propertyName)
    {
        var propertySymbol = this.codeAnalyzer.TryGetMember(
            qualifiedType,
            propertyName,
            x => x.OfType<IPropertySymbol>().FirstOrDefault());
        if (propertySymbol != null)
        {
            return R.Success(QualifiedSymbolFactory.CreateProperty(this.codeAnalyzer, propertySymbol, propertyName));
        }

        return R.Error();
    }

    public QualifiedProperty GetAttachedDependencyProperty(QualifiedType qualifiedType, string propertyName)
    {
        var getMethodInfo = this.TryGetMethodSymbol(qualifiedType, "Get" + propertyName, 1, true);
        var setMethodInfo = this.TryGetMethodSymbol(qualifiedType, "Set" + propertyName, 2, true);
        var typeSymbol = getMethodInfo?.ReturnType ?? setMethodInfo?.Parameters[1].Type;
        if (typeSymbol != null)
        {
            return new QualifiedProperty(QualifiedSymbolFactory.CreateType(this.codeAnalyzer, typeSymbol), propertyName, getMethodInfo != null, setMethodInfo != null);
        }

        throw new MemberNotFoundException(qualifiedType.ToAliasQualifiedType(), propertyName);
    }

    public R<QualifiedField> TryGetField(QualifiedType qualifiedType, string fieldName, bool isStatic = false)
    {
        var fieldSymbol = this.codeAnalyzer.TryGetMember(
            qualifiedType,
            fieldName,
            x => x.OfType<IFieldSymbol>().FirstOrDefault(y => y.IsStatic == isStatic));
        if (fieldSymbol != null)
        {
            return R.Success(QualifiedSymbolFactory.CreateField(this.codeAnalyzer, fieldName, fieldSymbol));
        }

        return R.Error();
    }

    public QualifiedProperty GetIndexer(QualifiedType qualifiedType, IEnumerable<QualifiedType> parameterTypes)
    {
        var propertySymbol = this.codeAnalyzer.TryGetMember(
            qualifiedType,
            IndexerName,
            x => x.OfType<IPropertySymbol>()
            .FirstOrDefault(
                y => y.Parameters.Select(z => z.Type)
                    .SequenceEqual(
                        parameterTypes.Select(
                            a => this.codeAnalyzer.GetTypeSymbol(a)))));
        if (propertySymbol != null)
        {
            return QualifiedSymbolFactory.CreateProperty(this.codeAnalyzer, propertySymbol, IndexerName);
        }

        throw new MemberNotFoundException(qualifiedType.ToAliasQualifiedType(), IndexerName);
    }

    public string GetAliasQualifiedGenericType(QualifiedType qualifiedType, params QualifiedType[] typeParameters)
    {
        var typeSymbol = this.codeAnalyzer.GetTypeSymbol(qualifiedType) as INamedTypeSymbol;
        if (typeSymbol == null || !typeSymbol.IsGenericType)
        {
            throw new TypeNotFoundException($"The type: {qualifiedType.ToNamespaceQualifiedType()} was not found or wasn't a generic type.");
        }

        var stringBuilder = new StringBuilder(QualifiedSymbolHelper.ToAliasQualifiedTypeString(
            this.codeAnalyzer.GetAssemblyAliases(typeSymbol).FirstOrDefault() ?? QualifiedType.GlobalAlias,
            typeSymbol.ContainingNamespace.ToString(),
            typeSymbol.Name));
        stringBuilder.Append('<');
        return typeParameters.Aggregate(
            stringBuilder,
            (builder, type) =>
            {
                builder.Append(type.ToAliasQualifiedType());
                builder.Append(',');
                builder.Append(' ');
                return builder;
            },
            builder => builder.Remove(builder.Length - 2, 2).Append('>').ToString());
    }

    private IMethodSymbol? TryGetMethodSymbol(QualifiedType qualifiedType, string methodName, int numberOfParameters, bool isStatic)
    {
        return this.codeAnalyzer.TryGetMember(qualifiedType, methodName, x => x.OfType<IMethodSymbol>()
            .FirstOrDefault(x => x.IsStatic == isStatic && x.Parameters.Length == numberOfParameters));
    }
}