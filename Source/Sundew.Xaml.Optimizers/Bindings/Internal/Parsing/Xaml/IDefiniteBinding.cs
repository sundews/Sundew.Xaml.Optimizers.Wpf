// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefiniteBinding.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal interface IDefiniteBinding : IBinding
{
    int Id { get; }

    bool IsEnabled { get; }

    BindingAssignment BindingAssignment { get; }

    bool IsBindingToTargetDataContext { get; }
}