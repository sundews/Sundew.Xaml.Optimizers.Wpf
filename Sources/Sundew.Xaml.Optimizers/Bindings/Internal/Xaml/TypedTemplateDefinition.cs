// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypedTemplateDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System.Xml.Linq;

internal class TypedTemplateDefinition : ITemplateDefinition
{
    public TypedTemplateDefinition(XName fullName, string typePropertyName)
    {
        this.FullName = fullName;
        this.TypePropertyName = typePropertyName;
    }

    public XName FullName { get; }

    public string TypePropertyName { get; }
}