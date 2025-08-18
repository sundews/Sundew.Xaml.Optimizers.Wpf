// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class XamlType
{
    public XamlType(string namespacePrefix, string typeName)
    {
        this.NamespacePrefix = namespacePrefix;
        this.TypeName = typeName;
    }

    public string NamespacePrefix { get; }

    public string TypeName { get; }

    public override string ToString()
    {
        return $"{this.NamespacePrefix}:{this.TypeName}";
    }
}