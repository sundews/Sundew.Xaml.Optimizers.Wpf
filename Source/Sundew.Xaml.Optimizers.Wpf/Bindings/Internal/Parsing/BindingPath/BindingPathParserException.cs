// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPathParserException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using System;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

internal class BindingPathParserException : Exception
{
    public BindingPathParserException(BindingPathError bindingPathError, Lexeme<__>? lexeme)
        : base($"Error: {bindingPathError} when parsing {lexeme?.ToString() ?? "<none>"}")
    {
        this.BindingPathError = bindingPathError;
        this.Lexeme = lexeme;
    }

    public BindingPathError BindingPathError { get; }

    public Lexeme<__>? Lexeme { get; }
}