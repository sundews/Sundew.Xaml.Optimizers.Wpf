// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingDispatcher.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;
using System.Windows.Threading;

internal class BindingDispatcher : IBindingDispatcher
{
    private readonly Dispatcher dispatcher;

    public BindingDispatcher(Dispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public bool HasAccess => this.dispatcher.CheckAccess();

    public void InvokeAsync(Action action)
    {
        this.dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, action);
    }
}