// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingSourceProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;

using System.Collections.Generic;
using Sundew.Base;
using Sundew.Base.Text;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class BindingSourceProvider
{
    private readonly TypeResolver typeResolver;
    private readonly Dictionary<BindingSource, Dictionary<string, BindingSource>> bindingSources = new Dictionary<BindingSource, Dictionary<string, BindingSource>>();
    private readonly Dictionary<string, int> bindingSourceNameIds = new Dictionary<string, int>();

    public BindingSourceProvider(TypeResolver typeResolver)
    {
        this.typeResolver = typeResolver;
    }

    public R<BindingSource, BindingSource> GetOrAddProperty(BindingSource bindingSource, string propertyName, bool acceptsSharedSource)
    {
        var suggestedName = bindingSource.SourceType.TypeName.Uncapitalize() + propertyName.Capitalize();
        var propertyType = this.typeResolver.GetProperty(bindingSource.SourceType, propertyName);
        if (acceptsSharedSource)
        {
            BindingSource requestedBindingSource;
            if (!this.bindingSources.TryGetValue(bindingSource, out var propertyNameDictionary))
            {
                requestedBindingSource = new BindingSource(propertyType.Type, suggestedName);
                this.bindingSources.Add(
                    bindingSource,
                    new Dictionary<string, BindingSource> { { propertyName, requestedBindingSource } });
                return R.Success(requestedBindingSource);
            }

            if (!propertyNameDictionary.TryGetValue(propertyName, out requestedBindingSource))
            {
                requestedBindingSource = new BindingSource(propertyType.Type, suggestedName);
                propertyNameDictionary.Add(propertyName, requestedBindingSource);
                return R.Success(requestedBindingSource);
            }

            return R.Error(requestedBindingSource);
        }

        return R.Success(this.GetUniqueBindingSource(propertyType.Type, suggestedName));
    }

    public R<BindingSource, BindingSource> GetOrAddIndexer(BindingSource bindingSource, IEnumerable<QualifiedType> indexerParameters)
    {
        var suggestedName = $"{bindingSource.SourceType.TypeName.Uncapitalize()}Indexer";
        var propertyType = this.typeResolver.GetIndexer(bindingSource.SourceType, indexerParameters);

        return R.Success(this.GetUniqueBindingSource(propertyType.Type, suggestedName));
    }

    public BindingSource AddElement(QualifiedType qualifiedType, string elementName)
    {
        var suggestedName = $"{qualifiedType.TypeName.Uncapitalize()}{elementName.Capitalize()}";
        return this.GetUniqueBindingSource(qualifiedType, suggestedName);
    }

    public BindingSource AddDataContext(QualifiedType qualifiedType)
    {
        return this.GetUniqueBindingSource(qualifiedType, qualifiedType.TypeName.Uncapitalize());
    }

    private BindingSource GetUniqueBindingSource(QualifiedType qualifiedType, string suggestedName)
    {
        var result = this.bindingSourceNameIds.TryGetValue(suggestedName, out var lastId);
        if (!result)
        {
            this.bindingSourceNameIds.Add(suggestedName, 1);
            return new BindingSource(qualifiedType, suggestedName + "1");
        }

        lastId++;
        this.bindingSourceNameIds[suggestedName] = lastId;
        return new BindingSource(qualifiedType, suggestedName + lastId);
    }
}