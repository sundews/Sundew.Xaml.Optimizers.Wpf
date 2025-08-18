// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateQueue.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System.Collections.Generic;

/// <summary>Updates pending dependency properties.</summary>
public sealed class UpdateQueue
{
    // private readonly HashSet<IBindingControl> bindingControlSet = new HashSet<IBindingControl>();
    private readonly Queue<IBindingControl> bindingControlQueue = new Queue<IBindingControl>();

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => this.bindingControlQueue.Count == 0;

    /// <summary>
    /// Enqueues the specified binding control.
    /// </summary>
    /// <param name="bindingControl">The binding control.</param>
    /// <returns>A dequeuer.</returns>
    public bool TryEnqueue(IBindingControl bindingControl)
    {
        /*if (this.bindingControlSet.Contains(bindingControl))
        {
            return false;
        }

        this.bindingControlSet.Add(bindingControl);*/

        if (this.bindingControlQueue.Contains(bindingControl))
        {
            return false;
        }

        this.bindingControlQueue.Enqueue(bindingControl);
        return true;
    }

    /// <summary>
    /// Gets the try dequeuer.
    /// </summary>
    /// <returns>A dequeuer.</returns>
    public Dequeuer GetDequeuer()
    {
        this.TryPeek(out var bindingControl);
        return new Dequeuer(this, bindingControl);
    }

    internal void Remove(IBindingControl bindingControl)
    {
        this.bindingControlQueue.Dequeue();
        //// this.bindingControlSet.Remove(bindingControl);
    }

    private bool TryPeek(out IBindingControl bindingControl)
    {
#if WINDOWS_UWP
            return this.bindingControlQueue.TryPeek(out bindingControl);
#else
        if (this.bindingControlQueue.Count > 0)
        {
            bindingControl = this.bindingControlQueue.Peek();
            return true;
        }

        bindingControl = null;
        return false;
#endif
    }
}

/// <summary>
/// A dequeuer that dequeues an element when disposed.
/// </summary>
#pragma warning disable SA1201 // Elements should appear in the correct order
public readonly ref struct Dequeuer
#pragma warning restore SA1201 // Elements should appear in the correct order
{
    private readonly UpdateQueue updateQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dequeuer" /> struct.
    /// </summary>
    /// <param name="updateQueue">The update queue.</param>
    /// <param name="bindingControl">The binding control.</param>
    public Dequeuer(UpdateQueue updateQueue, IBindingControl bindingControl)
    {
        this.updateQueue = updateQueue;
        this.Value = bindingControl;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public IBindingControl Value { get; }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        if (this.Value != null)
        {
            this.updateQueue.Remove(this.Value);
        }
    }
}