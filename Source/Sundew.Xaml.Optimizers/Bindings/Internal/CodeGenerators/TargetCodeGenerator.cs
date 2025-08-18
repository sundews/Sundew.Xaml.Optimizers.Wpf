// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;

using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal static class TargetCodeGenerator
{
    public static string GetTarget(QualifiedType elementType, string elementName, bool hasCodeBehind)
    {
        if (hasCodeBehind)
        {
            return $"this.Root.{elementName}";
        }

        return $@"({elementType.ToAliasQualifiedType()})this.Root.FindName(""{elementName}"")";
    }

    public static string GetTargetLambda(QualifiedType elementType, string elementName, bool hasCodeBehind)
    {
        if (hasCodeBehind)
        {
            return $"r => r.{elementName}";
        }

        return $@"r => ({elementType.ToAliasQualifiedType()})r.FindName(""{elementName}"")";
    }
}