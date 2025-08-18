// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementNotFoundException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System;

internal class ElementNotFoundException : Exception
{
    public ElementNotFoundException(string message)
        : base(message)
    {
    }
}