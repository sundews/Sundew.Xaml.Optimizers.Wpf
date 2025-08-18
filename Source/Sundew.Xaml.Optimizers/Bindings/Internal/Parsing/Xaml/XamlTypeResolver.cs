// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlTypeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class XamlTypeResolver
{
    private const string XmlNamespaceText = "XmlNamespace";
    private const string AssemblyText = "Assembly";
    private const string NamespaceText = "Namespace";
    private const string TypeNameText = "TypeName";
    private static readonly Regex XamlTypeNameRegex = new Regex(@"(.+d:Type=)?({x:Type )?(?<XmlNamespace>\w+):(?<TypeName>\w+)(})?(.+)?");
    private static readonly Regex NamespaceAssemblyRegex = new Regex("^clr-namespace:(?<Namespace>[^;]+)(;assembly=(?<Assembly>[^;]+))?$");
    private readonly string assemblyName;
    private readonly List<XAttribute> xNamespaces;
    private readonly IReadOnlyList<IAssemblyReference> assemblyReferences;
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> xamlTypeNamespaces;
    private readonly string defaultXmlNamespace;

    public XamlTypeResolver(
        string assemblyName,
        List<XAttribute> xNamespaces,
        IReadOnlyList<IAssemblyReference> assemblyReferences,
        Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> xamlTypeNamespaces)
    {
        this.assemblyName = assemblyName;
        this.xNamespaces = xNamespaces;
        this.assemblyReferences = assemblyReferences;
        this.xamlTypeNamespaces = xamlTypeNamespaces;
        this.defaultXmlNamespace = this.xNamespaces.First(x => x.Name.Namespace == XNamespace.None).Value;
    }

    public static XamlTypeResolver FromXDocument(
        XDocument xDocument,
        string assemblyName,
        IReadOnlyList<IAssemblyReference> assemblyReferences,
        Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> xamlTypeNamespaces)
    {
        return new XamlTypeResolver(assemblyName, xDocument.Root.Attributes().Where(x => x.IsNamespaceDeclaration).ToList(), assemblyReferences, xamlTypeNamespaces);
    }

    public R<QualifiedType> GetQualifiedType(XamlType xamlType)
    {
        return this.GetQualifiedType(xamlType.NamespacePrefix, xamlType.TypeName);
    }

    public R<QualifiedType> Parse(string xamlType)
    {
        var typeNameMatch = XamlTypeNameRegex.Match(xamlType);
        if (typeNameMatch.Success)
        {
            var namespacePrefix = typeNameMatch.Groups[XmlNamespaceText].Value;
            return this.GetQualifiedType(namespacePrefix, typeNameMatch.Groups[TypeNameText].Value);
        }

        return default;
    }

    public R<QualifiedType> Parse(XName xName)
    {
        var namespaceAssemblyMatch = NamespaceAssemblyRegex.Match(xName.Namespace.NamespaceName);
        if (namespaceAssemblyMatch.Success)
        {
            var assemblyGroup = namespaceAssemblyMatch.Groups[AssemblyText];
            var assembly = assemblyGroup.Success ? assemblyGroup.Value : this.assemblyName;
            return R.Success(new QualifiedType(this.FindAssemblyAlias(assembly), assembly, namespaceAssemblyMatch.Groups[NamespaceText].Value, xName.LocalName));
        }

        var xmlNamespace = string.IsNullOrEmpty(xName.NamespaceName) ? this.defaultXmlNamespace : xName.Namespace.ToString();
        if (this.xamlTypeNamespaces.Value.TryGetValue(xmlNamespace, out var dictionary))
        {
            if (dictionary.TryGetValue(xName.LocalName, out var sourceCodeName))
            {
                return R.Success(new QualifiedType(this.FindAssemblyAlias(sourceCodeName.AssemblyName), sourceCodeName.AssemblyName, sourceCodeName.Name, xName.LocalName));
            }
        }

        return R.Error();
    }

    private string FindAssemblyAlias(string assembly)
    {
        return this.assemblyReferences.FirstOrDefault(x => x.Name == assembly)?.Aliases.FirstOrDefault() ?? QualifiedType.GlobalAlias;
    }

    private R<QualifiedType> GetQualifiedType(string namespacePrefix, string typeName)
    {
        var attribute = this.xNamespaces.Find(x => x.Name.LocalName == namespacePrefix);
        if (attribute != null)
        {
            var namespaceAssemblyMatch = NamespaceAssemblyRegex.Match(attribute.Value);
            if (namespaceAssemblyMatch.Success)
            {
                var assemblyGroup = namespaceAssemblyMatch.Groups[AssemblyText];
                var assembly = assemblyGroup.Success ? assemblyGroup.Value : this.assemblyName;
                {
                    return R.Success(new QualifiedType(this.FindAssemblyAlias(assembly), assembly, namespaceAssemblyMatch.Groups[NamespaceText].Value, typeName));
                }
            }
        }

        return R.Error();
    }
}