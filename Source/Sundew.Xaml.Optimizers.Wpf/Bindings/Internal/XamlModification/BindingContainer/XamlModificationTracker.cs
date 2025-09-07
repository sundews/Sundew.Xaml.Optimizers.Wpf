// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlModificationTracker.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class XamlModificationTracker
{
    private readonly Dictionary<XElement, List<BindingXamlModification>> xamlModifications = new Dictionary<XElement, List<BindingXamlModification>>();

    public required XElement ModificationsRootElement { get; set; }

    public IEnumerable<BindingXamlModifications> XamlModifications => this.xamlModifications.Select(x => new BindingXamlModifications(x.Key, x.Value));

    public void Add(XElement targetElement, BindingXamlModification bindingXamlModification)
    {
        if (!this.xamlModifications.TryGetValue(targetElement, out var xamlChanges))
        {
            xamlChanges = new List<BindingXamlModification>();
            this.xamlModifications.Add(targetElement, xamlChanges);
        }

        xamlChanges.Add(bindingXamlModification);
    }
}