// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementBindingPair.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;

internal readonly struct ElementBindingPair
{
    public ElementBindingPair(ElementBindingSourceNode elementBindingSourceNode, List<IBinding> bindings)
    {
        this.ElementBindingSourceNode = elementBindingSourceNode;
        this.Bindings = bindings;
    }

    public ElementBindingSourceNode ElementBindingSourceNode { get; }

    public List<IBinding> Bindings { get; }
}