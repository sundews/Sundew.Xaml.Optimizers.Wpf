// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Engine.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
using System.Windows;
#endif

/// <summary>
/// The data binding engine.
/// </summary>
public class Engine
{
    private const int ProcessCrossThreadUpdateTime = 50000;
    private readonly IBindingDispatcher bindingDispatcher;
    private readonly ConcurrentQueue<IBindingControl> updateQueue = new ConcurrentQueue<IBindingControl>();

    internal Engine(IBindingDispatcher bindingDispatcher)
    {
        this.bindingDispatcher = bindingDispatcher;
    }

    /// <summary>Updates the specified data binding state.</summary>
    /// <param name="bindingControl">The binding control.</param>
    public void Update(IBindingControl bindingControl)
    {
        if (this.bindingDispatcher.HasAccess)
        {
            bindingControl.UpdateTargetValue();
            return;
        }

        if (bindingControl.IsUpdatePending)
        {
            return;
        }

        bindingControl.IsUpdatePending = true;
        var shouldDispatch = this.updateQueue.IsEmpty;
        this.updateQueue.Enqueue(bindingControl);
        if (shouldDispatch)
        {
            this.bindingDispatcher.InvokeAsync(this.PerformUpdate);
        }
    }

    private void PerformUpdate()
    {
        var shouldRetrigger = false;
        try
        {
            var timestamp = DateTime.UtcNow.Ticks;
            while (this.updateQueue.TryDequeue(out var bindingControl))
            {
                bindingControl.IsUpdatePending = false;
                bindingControl.UpdateTargetValue();

                if (DateTime.UtcNow.Ticks - timestamp > ProcessCrossThreadUpdateTime)
                {
                    shouldRetrigger = !this.updateQueue.IsEmpty;
                    break;
                }
            }
        }
        finally
        {
            if (shouldRetrigger)
            {
                this.bindingDispatcher.InvokeAsync(this.PerformUpdate);
            }
        }
    }
}