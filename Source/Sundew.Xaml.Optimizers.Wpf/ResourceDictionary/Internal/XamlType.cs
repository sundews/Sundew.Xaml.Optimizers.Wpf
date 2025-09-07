// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlType.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.ResourceDictionary.Internal;

using System.Xml.Linq;

internal class XamlType
{
    public XamlType(string prefix, XNamespace @namespace, string name)
    {
        this.Prefix = prefix;
        this.Namespace = @namespace;
        this.Name = name;
    }

    public string Prefix { get; }

    public XNamespace Namespace { get; }

    public string Name { get; }

    public static XamlType? TryParse(string? xamlType)
    {
        if (xamlType == null)
        {
            return null;
        }

        var parts = xamlType.Split('|');
        if (parts.Length == 3)
        {
            return new XamlType(parts[0], XNamespace.Get(parts[1]), parts[2]);
        }

        return null;
    }
}