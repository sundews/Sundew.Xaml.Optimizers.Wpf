// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlElementNameProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class XamlElementNameProvider
{
    private readonly XamlElementNameResolver xamlElementNameResolver;
    private int identifier = 1;

    public XamlElementNameProvider(XamlElementNameResolver xamlElementNameResolver)
    {
        this.xamlElementNameResolver = xamlElementNameResolver;
    }

    public string GetName(IBinding binding)
    {
        var elementName = binding.TargetElementName;
        if (string.IsNullOrEmpty(elementName))
        {
            return this.GetName(binding.TargetElement);
        }

        return elementName;
    }

    public string GetName(XElement xElement)
    {
        var name = this.xamlElementNameResolver.Resolve(xElement);
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        var suggestedName = xElement.Name.LocalName + this.identifier++;
        while (!this.xamlElementNameResolver.TryAddName(suggestedName, xElement))
        {
            suggestedName = xElement.Name.LocalName + this.identifier++;
        }

        return suggestedName;
    }

    public XAttribute TryGetNameAttribute(XElement xElement)
    {
        return this.xamlElementNameResolver.TryGetNameAttribute(xElement);
    }

    public void Reset()
    {
        this.identifier = 1;
    }
}