// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlModifications.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

internal class BindingXamlModifications : IReadOnlyList<BindingXamlModification>
{
    private readonly IReadOnlyList<BindingXamlModification> xamlChanges;

    public BindingXamlModifications(XElement targetElement, IReadOnlyList<BindingXamlModification> xamlChanges)
    {
        this.TargetElement = targetElement;
        this.xamlChanges = xamlChanges;
    }

    public XElement TargetElement { get; }

    public int Count => this.xamlChanges.Count;

    public BindingXamlModification this[int index] => this.xamlChanges[index];

    public IEnumerator<BindingXamlModification> GetEnumerator()
    {
        return this.xamlChanges.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}