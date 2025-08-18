// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingDispatcher.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;

internal interface IBindingDispatcher
{
    bool HasAccess { get; }

    void InvokeAsync(Action action);
}