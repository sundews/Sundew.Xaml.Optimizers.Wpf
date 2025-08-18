// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIndexerExpression.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

using System.Collections.Generic;

internal interface IIndexerExpression : IBindingPathExpression
{
    IReadOnlyList<Literal> Literals { get; }
}