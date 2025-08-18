// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalValue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal class AdditionalValue
{
    public AdditionalValue(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}