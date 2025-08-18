// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedBindingContainer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;

using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class GeneratedBindingContainer
{
    public GeneratedBindingContainer(
        string outputPath,
        QualifiedType bindingConnectorType,
        string sourceCode)
    {
        this.OutputPath = outputPath;
        this.BindingConnectorType = bindingConnectorType;
        this.SourceCode = sourceCode;
    }

    public string SourceCode { get; }

    public string OutputPath { get; }

    public QualifiedType BindingConnectorType { get; }
}