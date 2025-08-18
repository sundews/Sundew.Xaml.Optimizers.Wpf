// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

/// <summary>Contains information about the token.</summary>
public enum TokenInfo
{
    /// <summary>The token type.</summary>
    TokenType,

    /// <summary>The white space.</summary>
    WhiteSpace,

    /// <summary>The end.</summary>
    End,
}