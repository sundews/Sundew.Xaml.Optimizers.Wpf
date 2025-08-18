// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPathError{TTokenType}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

internal class BindingPathError<TTokenType>
{
    public BindingPathError(BindingPathError bindingPathError, Lexeme<TTokenType>? lexeme)
    {
        this.Error = bindingPathError;
        this.Lexeme = lexeme;
    }

    public BindingPathError Error { get; }

    public Lexeme<TTokenType>? Lexeme { get; }
}