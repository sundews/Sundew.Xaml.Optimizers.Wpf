// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssignmentCompatibilityAssessor.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class TypeAssignmentCompatibilityAssessor
{
    private readonly CodeAnalyzer codeAnalyzer;

    public TypeAssignmentCompatibilityAssessor(CodeAnalyzer codeAnalyzer)
    {
        this.codeAnalyzer = codeAnalyzer;
    }

    public TypeAssignmentCompatibility AssessTypeCompatibility(in QualifiedType target, in QualifiedType source)
    {
        var targetSymbol = this.codeAnalyzer.GetTypeSymbol(target);
        var sourceSymbol = this.codeAnalyzer.GetTypeSymbol(source);
        var result = TypeAssignmentCompatibility.None;
        if (targetSymbol == null || sourceSymbol == null)
        {
            return result;
        }

        var sourceToTargetResult = this.codeAnalyzer.Compilation.ClassifyCommonConversion(sourceSymbol, targetSymbol);
        var targetToSourceResult = this.codeAnalyzer.Compilation.ClassifyCommonConversion(targetSymbol, sourceSymbol);
        if (sourceToTargetResult.IsImplicit)
        {
            result |= TypeAssignmentCompatibility.SourceToTarget;
        }

        if (targetToSourceResult.IsImplicit)
        {
            result |= TypeAssignmentCompatibility.TargetToSource;
        }

        return result;
    }
}