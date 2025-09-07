// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingPathLexicalAnalyzer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;

using System.Linq;
using System.Text.RegularExpressions;
using Sundew.Base;

internal class BindingPathLexicalAnalyzer
{
    private const string Tokens = "Tokens";
    private static readonly Regex Tokenizer = new Regex(@"(?<Tokens>[\w\ ]+|\.|\(|\)|\[|\]|\:|\,)*");

    public R<Lexemes<__>> Analyze(string input)
    {
        var match = Tokenizer.Match(input);
        if (match.Success)
        {
            var lexemes = match.Groups[Tokens].Captures.Cast<Capture>().Select(x => new Lexeme<__>(x.Value, __._, x.Index)).ToList();
            lexemes.Add(new Lexeme<__>(string.Empty, TokenInfo.End, input.Length));
            return R.Success(new Lexemes<__>(lexemes));
        }

        return R.Error();
    }
}