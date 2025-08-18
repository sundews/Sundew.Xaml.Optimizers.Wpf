// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StackExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;

using System.Collections.Generic;
using Sundew.Base.Collections;

internal static class StackExtensions
{
    public static bool TryPeek<TItem>(this Stack<TItem> stack, out TItem item)
    {
        if (stack.IsEmpty())
        {
            item = default;
            return false;
        }

        item = stack.Peek();
        return true;
    }
}