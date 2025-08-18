// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNotFoundException.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Exceptions;

using System;

internal class MemberNotFoundException : Exception
{
    public MemberNotFoundException(string type, string name)
        : base($"The member could not be found on type: {type}: member {name}")
    {
    }
}