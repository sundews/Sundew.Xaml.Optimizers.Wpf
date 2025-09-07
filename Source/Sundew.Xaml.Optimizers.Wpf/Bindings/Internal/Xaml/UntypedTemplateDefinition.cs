// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UntypedTemplateDefinition.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System.Xml.Linq;

internal class UntypedTemplateDefinition : ITemplateDefinition
{
    public UntypedTemplateDefinition(XName fullName)
    {
        this.FullName = fullName;
    }

    public XName FullName { get; }
}