// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lexeme.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

/// <summary>Contains information about a token, its type and position.</summary>
/// <typeparam name="TTokenType">The type of the token.</typeparam>
public sealed class Lexeme<TTokenType>
{
    /// <summary>Initializes a new instance of the <see cref="Lexeme{TTokenType}"/> class.</summary>
    /// <param name="token">The token.</param>
    /// <param name="tokenType">Type of the token.</param>
    /// <param name="position">The position.</param>
    public Lexeme(string token, TTokenType? tokenType, int position)
        : this(token, TokenInfo.TokenType, tokenType, position)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Lexeme{TTokenType}"/> class.</summary>
    /// <param name="token">The token.</param>
    /// <param name="tokenInfo">The token info.</param>
    /// <param name="position">The position.</param>
    public Lexeme(string token, TokenInfo tokenInfo, int position)
        : this(token, tokenInfo, default, position)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Lexeme{TTokenType}"/> class.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="tokenInfo">The token info.</param>
    /// <param name="tokenType">Type of the token.</param>
    /// <param name="position">The position.</param>
    public Lexeme(string token, TokenInfo tokenInfo, TTokenType? tokenType, int position)
    {
        this.Token = token;
        this.TokenInfo = tokenInfo;
        this.TokenType = tokenType;
        this.Position = position;
    }

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <value>
    /// The token.
    /// </value>
    public string Token { get; }

    /// <summary>Gets the token information.</summary>
    /// <value>The token information.</value>
    public TokenInfo TokenInfo { get; }

    /// <summary>
    /// Gets the type of the token.
    /// </summary>
    /// <value>
    /// The type of the token.
    /// </value>
    public TTokenType? TokenType { get; }

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public int Position { get; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return this.Token;
    }
}