// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeNotFoundException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Exceptions;

using System;

internal class TypeNotFoundException : Exception
{
    public TypeNotFoundException(string message)
        : base(message)
    {
    }
}