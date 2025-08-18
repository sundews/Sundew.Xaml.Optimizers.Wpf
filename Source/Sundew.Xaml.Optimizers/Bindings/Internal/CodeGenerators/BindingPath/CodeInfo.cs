// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;

using System.Text;

internal sealed class CodeInfo
{
    public CodeInfo(StringBuilder bindingPathSourceCodeBuilder, BindingSource bindingSource)
    {
        this.BindingPathSourceCodeBuilder = bindingPathSourceCodeBuilder;
        this.BindingSource = bindingSource;
    }

    public StringBuilder BindingPathSourceCodeBuilder { get; }

    public BindingSource BindingSource { get; }
}