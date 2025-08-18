// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingConnector.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations;

using System;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
#endif
using Sundew.Xaml.Optimizations.Bindings;

/// <summary>Base class for implementing a binding connector.</summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <seealso cref="IBindingConnector" />
public abstract class BindingConnector<TRoot> : MarkupExtension, IBindingConnector
    where TRoot : DependencyObject
{
    private readonly List<IBinding> bindings = new List<IBinding>();
    private readonly List<int> phases = new List<int>();
    private readonly BindingConnectorController bindingConnectorController;
    private PhaseContext phaseContext;

    /// <summary>Initializes a new instance of the <see cref="BindingConnector{TRoot}"/> class.</summary>
    protected BindingConnector()
    {
        this.bindingConnectorController = new BindingConnectorController(this);
    }

    /// <summary>Gets the root.</summary>
    /// <value>The root.</value>
    public TRoot Root { get; private set; }

    DependencyObject IBindingConnector.Root => this.Root;

    /// <summary>Connects the specified target dependency object.</summary>
    /// <param name="root">The target dependency object.</param>
    public void Connect(DependencyObject root)
    {
        this.Root = (TRoot)root;
        this.bindingConnectorController.Initialize(this.Root);
#if WINDOWS_UWP
        this.phases.Clear();
        this.OnConnect();
        this.Reconnect();
#else

        void OnRootInitialized(object sender, EventArgs args)
        {
            if (this.Root is FrameworkElement frameworkElement)
            {
                frameworkElement.Initialized -= OnRootInitialized;
            }

            if (this.Root is FrameworkContentElement frameworkContentElement)
            {
                frameworkContentElement.Initialized -= OnRootInitialized;
            }

            this.phases.Clear();
            this.OnConnect();
            this.Reconnect();
        }

        if (this.Root is FrameworkElement frameworkElement)
        {
            frameworkElement.Initialized += OnRootInitialized;
        }

        if (this.Root is FrameworkContentElement frameworkContentElement)
        {
            frameworkContentElement.Initialized += OnRootInitialized;
        }
#endif
    }

    /// <summary>
    /// Reconnects this instance.
    /// </summary>
    public void Reconnect()
    {
        var currentPhase = 0;
        foreach (var binding in this.bindings)
        {
            binding.Connect(currentPhase);
        }

        if (this.phases.Any())
        {
            this.phaseContext = new PhaseContext(this.phases, this.Root.Dispatcher, this.bindings);
            this.Root.Dispatcher.BeginInvoke(new Action<PhaseContext>(phaseContext => phaseContext.Update()), this.phaseContext);
        }
    }

    /// <summary>Disconnects this instance.</summary>
    public void Disconnect()
    {
        this.phaseContext?.Cancel();
        foreach (var binding in this.bindings)
        {
            binding.Disconnect();
        }
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        this.phaseContext?.Cancel();
        this.bindingConnectorController.UnInitialize(this.Root);
        foreach (var bindindg in this.bindings)
        {
            bindindg.Dispose();
        }

        this.bindings.Clear();
    }

    /// <summary>Gets the data context.</summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <param name="getData">The get data.</param>
    /// <returns>The data context.</returns>
    public BindingContext<TRoot, TData> GetDataContext<TData>(Func<TRoot, TData> getData)
    {
        var dataContext = new BindingContext<TRoot, TData>(this.Root, getData, true);
        this.bindings.Add(dataContext);
        return dataContext;
    }

    /// <summary>Gets the element context.</summary>
    /// <typeparam name="TNestedView">The type of the nested view.</typeparam>
    /// <param name="getView">The get view.</param>
    /// <returns>An element context.</returns>
    public BindingContext<TRoot, TNestedView> GetElementContext<TNestedView>(Func<TRoot, TNestedView> getView)
        where TNestedView : DependencyObject
    {
        var elementContext = new BindingContext<TRoot, TNestedView>(this.Root, getView, false);
        this.bindings.Add(elementContext);
        return elementContext;
    }

    /// <summary>Refreshes this instance.</summary>
    public void Refresh()
    {
        this.OnRefresh();
        foreach (var context in this.bindings)
        {
            context.Refresh();
        }
    }

    /// <summary>Connects this instance.</summary>
    protected abstract void OnConnect();

    /// <summary>Called when [refresh].</summary>
    protected virtual void OnRefresh()
    {
    }

    private class PhaseContext
    {
        private readonly List<int> phases;
        private readonly IList<IBinding> bindings;
        private readonly Dispatcher dispatcher;
        private int phaseIndex;
        private bool shouldCancel;

        public PhaseContext(List<int> phases, Dispatcher dispatcher, IList<IBinding> bindings)
        {
            this.phases = phases;
            this.bindings = bindings;
            this.dispatcher = dispatcher;
        }

        public void Update()
        {
            foreach (var binding in this.bindings)
            {
                if (this.shouldCancel)
                {
                    return;
                }

                binding.Connect(this.phases[this.phaseIndex]);
            }

            if (this.shouldCancel)
            {
                return;
            }

            this.phaseIndex++;
            if (this.phaseIndex >= this.phases.Count)
            {
                return;
            }

            this.dispatcher.BeginInvoke(new Action<PhaseContext>(phaseContext => phaseContext.Update()), this);
        }

        public void Cancel()
        {
            this.shouldCancel = true;
        }
    }
}