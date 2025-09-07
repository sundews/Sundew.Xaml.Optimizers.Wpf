// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlTypeBaseTypeSourceCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class XamlTypeBaseTypeSourceCodeGenerator
{
    private readonly IXDocumentProvider xDocumentProvider;
    private readonly XName xClassName;
    private readonly string assemblyName;
    private readonly IReadOnlyList<IAssemblyReference> assemblyReferences;
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> xamlTypeToSourceCodeTypes;

    public XamlTypeBaseTypeSourceCodeGenerator(
        IXDocumentProvider xDocumentProvider,
        XName xClassName,
        string assemblyName,
        IReadOnlyList<IAssemblyReference> assemblyReferences,
        Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> xamlTypeToSourceCodeTypes)
    {
        this.xDocumentProvider = xDocumentProvider;
        this.xClassName = xClassName;
        this.assemblyName = assemblyName;
        this.assemblyReferences = assemblyReferences;
        this.xamlTypeToSourceCodeTypes = xamlTypeToSourceCodeTypes;
    }

    public IEnumerable<string> GenerateXamlTypes()
    {
        var concurrentBag = new ConcurrentBag<string>();
        Parallel.ForEach(this.xDocumentProvider.FileReferences, (fileReference, state) =>
        {
            var xDocument = this.xDocumentProvider.Get(fileReference);
            if (xDocument.Root == null)
            {
                return;
            }

            var classNameAttribute = xDocument.Root.Attribute(this.xClassName);
            var xamlTypeResolver = XamlTypeResolver.FromXDocument(
                xDocument,
                this.assemblyName,
                this.assemblyReferences,
                this.xamlTypeToSourceCodeTypes);
            if (classNameAttribute != null)
            {
                GetNamespaceAndTypeName(classNameAttribute.Value, out var namespaceName, out var typeName);
                var baseType = xamlTypeResolver.Parse(xDocument.Root.Name);

                var sourceCode = @$"namespace {namespaceName}
{{
    public partial class {typeName} : {baseType.Value!.ToAliasQualifiedType()}
    {{
    }}
}}";
                concurrentBag.Add(sourceCode);
            }
        });

        return concurrentBag;
    }

    private static void GetNamespaceAndTypeName(string namespaceQualifiedType, out string namespaceName, out string typeName)
    {
        var lastDotIndex = namespaceQualifiedType.LastIndexOf('.');
        if (lastDotIndex > -1)
        {
            namespaceName = namespaceQualifiedType.Substring(0, lastDotIndex);
            typeName = namespaceQualifiedType.Substring(lastDotIndex + 1);
            return;
        }

        namespaceName = string.Empty;
        typeName = namespaceQualifiedType;
    }
}