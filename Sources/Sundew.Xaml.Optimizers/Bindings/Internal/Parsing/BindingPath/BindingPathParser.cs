// -------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPathParser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using System;
using System.Collections.Generic;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

internal class BindingPathParser
{
    private readonly BindingPathLexicalAnalyzer bindingPathLexicalAnalyzer;

    public BindingPathParser(BindingPathLexicalAnalyzer bindingPathLexicalAnalyzer)
    {
        this.bindingPathLexicalAnalyzer = bindingPathLexicalAnalyzer;
    }

    public R<IBindingPathExpression, BindingPathError<__>> Parse(string bindingPath)
    {
        try
        {
            var lexemesResult = this.bindingPathLexicalAnalyzer.Analyze(bindingPath);
            if (lexemesResult.IsError)
            {
                return R.Error(new BindingPathError<__>(BindingPathError.SyntaxError, null));
            }

            var lexemes = lexemesResult.Value;
            var bindingPathExpression = this.TryBindingPath(lexemes);
            if (bindingPathExpression.IsError)
            {
                return R.Error(new BindingPathError<__>(BindingPathError.SyntaxError, lexemes.Current));
            }

            if (lexemes.AcceptTokenType(TokenInfo.End))
            {
                return R.Success<IBindingPathExpression>(new DataContextSource());
            }

            return R.Error(new BindingPathError<__>(BindingPathError.EndMissing, lexemes.Current));
        }
        catch (BindingPathParserException e)
        {
            return R.Error(new BindingPathError<__>(e.BindingPathError, e.Lexeme));
        }
    }

    private static Exception CreateParseException(BindingPathError bindingPathError, Lexeme<__>? lexeme)
    {
        throw new BindingPathParserException(bindingPathError, lexeme);
    }

    private R<IBindingPathExpression> TryBindingPath(Lexemes<__> lexemes)
    {
        var lhs = this.PrimaryExpression(lexemes);
        return this.BindingPath(lexemes, lhs);
    }

    private R<IBindingPathExpression> BindingPath(Lexemes<__> lexemes, IBindingPathExpression lhs)
    {
        lhs = this.IndexerAccessor(lexemes, lhs);
        lhs = this.PropertyAccessor(lexemes, lhs);

        return lhs;
    }

    private IBindingPathExpression PrimaryExpression(Lexemes<__> lexemes)
    {
        if (lexemes.AcceptToken("."))
        {
            return new DataContextSource();
        }

        var indexerResult = this.Indexer(lexemes);
        if (indexerResult.IsSuccess)
        {
            return indexerResult.Value;
        }

        var dependencyPropertyResult = this.AttachedDependencyProperty(lexemes);
        if (dependencyPropertyResult.IsSuccess)
        {
            return dependencyPropertyResult.Value;
        }

        return this.Property(lexemes, false);
    }

    private IBindingPathExpression PropertyAccessor(Lexemes<__> lexemes, IBindingPathExpression lhs)
    {
        if (lexemes.AcceptToken("."))
        {
            var result = this.AttachedDependencyProperty(lexemes);
            if (result.IsSuccess)
            {
                result = this.Property(lexemes, true);
            }

            return this.BindingPath(lexemes, new PropertyAccessor(lhs, result));
        }

        return lhs;
    }

    private R<IPropertyExpression> AttachedDependencyProperty(Lexemes<__> lexemes)
    {
        if (lexemes.AcceptToken("("))
        {
            var xamlType = this.XamlType(lexemes);
            if (!xamlType.IsSuccess)
            {
                return R.Error();
            }

            lexemes.AcceptToken(".");
            if (!lexemes.AcceptTokenType(__._, out var propertyName))
            {
                return R.Error();
            }

            if (lexemes.AcceptToken(")"))
            {
                return R.Success<IPropertyExpression>(new AttachedDependencyProperty(xamlType.Value, propertyName));
            }
        }

        return R.Error();
    }

    private R<XamlType> XamlType(Lexemes<__> lexemes)
    {
        var namespacePrefix = string.Empty;
        if (!lexemes.AcceptTokenType(__._, out var identifier))
        {
            return R.Error();
        }

        if (lexemes.AcceptToken(":"))
        {
            namespacePrefix = identifier;
            if (!lexemes.AcceptTokenType(__._, out identifier))
            {
                return R.Error();
            }
        }

        return R.Success(new XamlType(namespacePrefix, identifier));
    }

    private IBindingPathExpression IndexerAccessor(Lexemes<__> lexemes, IBindingPathExpression lhs)
    {
        var indexerResult = this.Indexer(lexemes);
        if (indexerResult.IsSuccess)
        {
            return new IndexerAccessor(lhs, indexerResult.Value);
        }

        return lhs;
    }

    private R<IIndexerExpression> Indexer(Lexemes<__> lexemes)
    {
        if (lexemes.AcceptToken("["))
        {
            var literalList = new List<Literal>();
            this.LiteralList(lexemes, literalList);
            if (lexemes.AcceptToken("]"))
            {
                if (lexemes.AcceptTokenType(TokenInfo.End))
                {
                    lexemes.MoveToPrevious();
                    return R.Success<IIndexerExpression>(new Indexer(literalList));
                }

                return R.Success<IIndexerExpression>(new IndexerPart(literalList));
            }

            throw CreateParseException(BindingPathError.RightAngleBracketMissing, lexemes.Current);
        }

        return R.Error();
    }

    private void LiteralList(Lexemes<__> lexemes, List<Literal> literalList)
    {
        XamlType? castXamlType = null;
        if (lexemes.AcceptToken("("))
        {
            var castXamlTypeResult = this.XamlType(lexemes);
            if (!castXamlTypeResult.TryGet(out castXamlType))
            {
                throw CreateParseException(BindingPathError.XamlTypeMissing, lexemes.Current);
            }

            if (!lexemes.AcceptToken(")"))
            {
                throw CreateParseException(BindingPathError.RightParenthesisMissing, lexemes.Current);
            }
        }

        if (!lexemes.AcceptTokenType(__._, true, out var value))
        {
            throw CreateParseException(BindingPathError.ValueMissing, lexemes.Current);
        }

        literalList.Add(new Literal(castXamlType, value.Trim()));
        if (lexemes.AcceptToken(","))
        {
            this.LiteralList(lexemes, literalList);
        }
    }

    private R<IPropertyExpression> Property(Lexemes<__> lexemes, bool isRequired)
    {
        if (lexemes.AcceptTokenType(__._, out var value))
        {
            if (lexemes.AcceptTokenType(TokenInfo.End))
            {
                lexemes.MoveToPrevious();
                return R.Success<IPropertyExpression>(new Property(value));
            }

            return R.Success<IPropertyExpression>(new PropertyPart(value));
        }

        if (isRequired)
        {
            throw CreateParseException(BindingPathError.PropertyNameMissing, lexemes.Current);
        }

        return R.Error();
    }
}