// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashSetExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;

using System.Collections.Generic;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal static class HashSetExtensions
{
    public static bool TryAdd(this HashSet<string> hashSet, QualifiedType qualifiedType)
    {
        if (qualifiedType.UsesGlobalAlias)
        {
            return false;
        }

        return hashSet.Add(qualifiedType.AssemblyAlias);
    }
}