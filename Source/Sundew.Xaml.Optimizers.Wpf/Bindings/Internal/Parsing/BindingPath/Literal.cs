// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Literal.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class Literal
{
    public Literal(XamlType? type, string value)
    {
        this.Type = type;
        this.Value = value;
    }

    public XamlType? Type { get; }

    public string Value { get; }

    public override string ToString()
    {
        if (this.Type != null)
        {
            return $"({this.Type}){this.Value}";
        }

        return this.Value;
    }
}