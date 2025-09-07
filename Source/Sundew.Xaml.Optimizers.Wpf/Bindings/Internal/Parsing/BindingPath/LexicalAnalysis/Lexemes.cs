// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lexemes.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sundew.Base;

/// <summary>
/// Contains <see cref="Lexeme{TTokenType}"/>s for a parser.
/// </summary>
/// <typeparam name="TTokenType">The type of the token.</typeparam>
public sealed class Lexemes<TTokenType> : IEnumerable<Lexeme<TTokenType>>
{
    private readonly List<Lexeme<TTokenType>> lexemes;
    private int currentIndex = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexemes{TTokenType}"/> class.
    /// </summary>
    /// <param name="lexemes">The lexemes.</param>
    public Lexemes(List<Lexeme<TTokenType>> lexemes)
    {
        this.lexemes = lexemes;
    }

    /// <summary>
    /// Gets the current.
    /// </summary>
    /// <value>
    /// The current.
    /// </value>
    public Lexeme<TTokenType>? Current
    {
        get
        {
            if (this.currentIndex >= this.lexemes.Count)
            {
                return null;
            }

            return this.lexemes[this.currentIndex];
        }
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{Lexeme}"/>.</returns>
    public IEnumerator<Lexeme<TTokenType>> GetEnumerator()
    {
        return this.lexemes.GetEnumerator();
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Accepts the token from.
    /// </summary>
    /// <param name="tokenRegex">The token regex.</param>
    /// <param name="token">The token.</param>
    /// <returns>
    ///   <c>true</c> if the specified token regex matches the token, otherwise <c>false</c>.
    /// </returns>
    public bool AcceptTokenFrom(Regex tokenRegex, [NotNullWhen(true)] out string? token)
    {
        return this.AcceptTokenFrom(tokenRegex, true, out token);
    }

    /// <summary>
    /// Accepts the token from a regex.
    /// </summary>
    /// <param name="tokenRegex">The token regex.</param>
    /// <param name="ignoreWhitespace">if set to <c>true</c> white space will be TVoidd.</param>
    /// <param name="token">The token.</param>
    /// <returns>
    ///   <c>true</c> if the specified token regex matches the token, otherwise <c>false</c>.
    /// </returns>
    public bool AcceptTokenFrom(Regex tokenRegex, bool ignoreWhitespace, [NotNullWhen(true)] out string? token)
    {
        var ignoredWhiteSpace = this.TryIgnoreWhitespace(ignoreWhitespace, this.Current);
        if (!this.Current.HasValue())
        {
            token = null;
            return false;
        }

        var match = tokenRegex.Match(this.Current.Token);
        if (match.Success)
        {
            token = match.Value;
            this.MoveToNext();
            return true;
        }

        if (ignoredWhiteSpace)
        {
            this.MoveToPrevious();
        }

        token = null;
        return false;
    }

    /// <summary>
    /// Accepts the token from a registry.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="possibleTokens">The possible tokens.</param>
    /// <param name="result">The result.</param>
    /// <returns><c>true</c> if the specified lexeme registry contains the token, otherwise <c>false</c>.</returns>
    public bool AcceptTokenFrom<TResult>(ILexemeRegistry<TResult> possibleTokens, [NotNullWhen(true)] out TResult result)
    {
        return this.AcceptTokenFrom(possibleTokens, true, out result);
    }

    /// <summary>
    /// Accepts the token from.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="possibleTokens">The possible tokens.</param>
    /// <param name="ignoreWhitespace">if set to <c>true</c> white space will be TVoidd.</param>
    /// <param name="result">The result.</param>
    /// <returns>
    ///   <c>true</c> if the token was accepted, otherwise <c>false</c>.
    /// </returns>
    public bool AcceptTokenFrom<TResult>(
        ILexemeRegistry<TResult> possibleTokens,
        bool ignoreWhitespace,
        out TResult result)
    {
        var ignoredWhiteSpace = this.TryIgnoreWhitespace(ignoreWhitespace, this.Current);
        if (this.Current.HasValue() && possibleTokens.TryGet(this.Current.Token, out result))
        {
            this.MoveToNext();
            return true;
        }

        if (ignoredWhiteSpace)
        {
            this.MoveToPrevious();
        }

        result = default!;
        return false;
    }

    /// <summary>
    /// Accepts the token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="ignoreWhitespace">if set to <c>true</c> white space will be TVoidd.</param>
    /// <returns>
    ///   <c>true</c> if the specified token is accepted, otherwise <c>false</c>.
    /// </returns>
    public bool AcceptToken(string token, bool ignoreWhitespace = true)
    {
        var ignoredWhiteSpace = this.TryIgnoreWhitespace(ignoreWhitespace, this.Current);
        if (this.Current?.Token == token)
        {
            this.MoveToNext();
            return true;
        }

        if (ignoredWhiteSpace)
        {
            this.MoveToPrevious();
        }

        return false;
    }

    /// <summary>
    /// Accepts the type of the token.
    /// </summary>
    /// <param name="tokenType">Type of the token.</param>
    /// <returns>
    ///   <c>true</c> if the specified token type is accepted, otherwise <c>false</c>.
    /// </returns>
    public bool AcceptTokenType(TTokenType tokenType)
    {
        return this.AcceptTokenType(tokenType, true, out _);
    }

    /// <summary>
    /// Accepts the type of the token.
    /// </summary>
    /// <param name="tokenType">Type of the token.</param>
    /// <param name="token">The token.</param>
    /// <returns><c>true</c> if the specified token type is accepted, otherwise <c>false</c>.</returns>
    public bool AcceptTokenType(TTokenType tokenType, [NotNullWhen(true)] out string? token)
    {
        return this.AcceptTokenType(TokenInfo.TokenType, tokenType, true, true, out token);
    }

    /// <summary>
    /// Accepts the type of the token.
    /// </summary>
    /// <param name="tokenInfo">The token info.</param>
    /// <returns><c>true</c> if the specified token type is accepted, otherwise <c>false</c>.</returns>
    public bool AcceptTokenType(TokenInfo tokenInfo)
    {
        return this.AcceptTokenType(tokenInfo, true, out _);
    }

    /// <summary>
    /// Accepts the type of the token.
    /// </summary>
    /// <param name="tokenInfo">The token info.</param>
    /// <param name="token">The token.</param>
    /// <returns><c>true</c> if the specified token type is accepted, otherwise <c>false</c>.</returns>
    public bool AcceptTokenType(TokenInfo tokenInfo, [NotNullWhen(true)] out string? token)
    {
        return this.AcceptTokenType(tokenInfo, true, out token);
    }

    /// <summary>Accepts the type of the token.</summary>
    /// <param name="tokenType">Type of the token.</param>
    /// <param name="ignoreWhitespace">if set to <c>true</c> [ignore white space].</param>
    /// <param name="token">The token.</param>
    /// <returns><c>true</c> if the specified token type is accepted, otherwise <c>false</c>.</returns>
    public bool AcceptTokenType(TTokenType tokenType, bool ignoreWhitespace, [NotNullWhen(true)] out string? token)
    {
        return this.AcceptTokenType(TokenInfo.TokenType, tokenType, ignoreWhitespace, true, out token);
    }

    /// <summary>
    /// Accepts the type of the token.
    /// </summary>
    /// <param name="tokenInfo">The token info.</param>
    /// <param name="ignoreWhitespace">if set to <c>true</c> white space will be ignored.</param>
    /// <param name="token">The token.</param>
    /// <returns><c>true</c> if the specified token type is accepted, otherwise <c>false</c>.</returns>
    public bool AcceptTokenType(TokenInfo tokenInfo, bool ignoreWhitespace, [NotNullWhen(true)] out string? token)
    {
        return this.AcceptTokenType(tokenInfo, default, ignoreWhitespace, false, out token);
    }

    /// <summary>
    /// Moves to previous.
    /// </summary>
    /// <returns>The previous <see cref="Lexeme{TTokenType}"/>.</returns>
    public Lexeme<TTokenType> MoveToPrevious()
    {
        return this.lexemes[--this.currentIndex];
    }

    private bool AcceptTokenType(TokenInfo tokenInfo, TTokenType? tokenType, bool ignoreWhitespace, bool allowTokenTypeComparison, [NotNullWhen(true)] out string? token)
    {
        switch (tokenInfo)
        {
            case TokenInfo.WhiteSpace:
                ignoreWhitespace = false;
                break;
        }

        var ignoredWhiteSpace = this.TryIgnoreWhitespace(ignoreWhitespace, this.Current);
        var lexeme = this.Current;
        if ((allowTokenTypeComparison && lexeme.HasValue() && lexeme.TokenInfo == TokenInfo.TokenType && lexeme.TokenType.Equals(tokenType)) || lexeme?.TokenInfo == tokenInfo)
        {
            token = lexeme.Token;
            this.MoveToNext();
            return true;
        }

        if (ignoredWhiteSpace)
        {
            this.MoveToPrevious();
        }

        token = null;
        return false;
    }

    private bool TryIgnoreWhitespace(bool ignoreWhitespace, Lexeme<TTokenType>? lexeme)
    {
        if (ignoreWhitespace && lexeme?.TokenInfo == TokenInfo.WhiteSpace)
        {
            this.MoveToNext();
            return true;
        }

        return false;
    }

    private void MoveToNext()
    {
        this.currentIndex++;
    }
}