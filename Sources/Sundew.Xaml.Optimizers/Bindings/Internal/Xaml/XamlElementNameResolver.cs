// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlElementNameResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System.Collections.Generic;
using System.Xml.Linq;

internal class XamlElementNameResolver
{
    private const string NameText = "Name";
    private readonly XName xamlName;
    private readonly Dictionary<string, XElement> xamlNames = new Dictionary<string, XElement>();
    private readonly Dictionary<XElement, string> elementNames = new Dictionary<XElement, string>();

    public XamlElementNameResolver(XNamespace xamlNamespace)
    {
        this.xamlName = xamlNamespace + NameText;
    }

    public bool TryAddName(string name, XElement element)
    {
        if (this.xamlNames.ContainsKey(name))
        {
            return false;
        }

        this.elementNames.Add(element, name);
        this.xamlNames.Add(name, element);
        return true;
    }

    public string Resolve(XElement xElement)
    {
        if (this.elementNames.TryGetValue(xElement, out var name))
        {
            return name;
        }

        return null;
    }

    public XElement Resolve(string name)
    {
        if (this.xamlNames.TryGetValue(name, out var xElement))
        {
            return xElement;
        }

        return null;
    }

    public string TryRegisterName(XElement xElement)
    {
        var nameAttribute = this.TryGetNameAttribute(xElement);
        if (nameAttribute != null)
        {
            this.elementNames.Add(xElement, nameAttribute.Value);
            this.xamlNames.Add(nameAttribute.Value, xElement);
            return nameAttribute.Value;
        }

        return null;
    }

    public XAttribute TryGetNameAttribute(XElement xElement)
    {
        return xElement.Attribute(NameText) ?? xElement.Attribute(this.xamlName);
    }
}