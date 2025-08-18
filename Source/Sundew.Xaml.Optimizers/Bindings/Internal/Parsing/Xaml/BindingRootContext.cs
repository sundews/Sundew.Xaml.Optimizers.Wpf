// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingRootContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System.Collections.Generic;

internal sealed class BindingRootContext
{
    public BindingRootContext(bool hasCodeBehind)
    {
        this.HasCodeBehind = hasCodeBehind;
    }

    public Dictionary<string, ElementBindingPair> ElementBindingSources { get; } = new Dictionary<string, ElementBindingPair>();

    public List<IBinding> Bindings { get; } = new List<IBinding>();

    public bool HasCodeBehind { get; }
}