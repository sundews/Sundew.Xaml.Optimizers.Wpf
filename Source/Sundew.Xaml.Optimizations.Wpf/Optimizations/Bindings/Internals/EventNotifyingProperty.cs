// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventNotifyingProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif

internal class EventNotifyingProperty<TSource, TEventHandler> : INotifyingProperty<TSource>
{
    private readonly ISourceContext<TSource> sourceContext;
    private readonly Func<TSource, Action, TEventHandler> subscribe;
    private readonly Action<TSource, TEventHandler> unsubscribe;
    private TEventHandler eventHandler;
    private IBindingControl bindingControl;

    public EventNotifyingProperty(ISourceContext<TSource> sourceContext, Func<TSource, Action, TEventHandler> subscribe, Action<TSource, TEventHandler> unsubscribe)
    {
        this.sourceContext = sourceContext;
        this.subscribe = subscribe;
        this.unsubscribe = unsubscribe;
    }

    public void Initialize(IBindingControl bindingControl)
    {
        this.bindingControl = bindingControl;
    }

    public TSource TryUpdateSubscription(BindingMode bindingMode, TSource source)
    {
        if (DataBindingHelper.RequiresSourcePropertyChangeNotification(bindingMode))
        {
            var newSource = this.sourceContext.Source;
            if (!Equals(newSource, source))
            {
                this.Unsubscribe(source);
                source = newSource;
                if (!Equals(source, default))
                {
                    this.eventHandler = this.subscribe(source, this.OnSourcePropertyChanged);
                }
            }
        }

        return source;
    }

    public void Unsubscribe(TSource source)
    {
        if (source != null && this.eventHandler != null)
        {
            this.unsubscribe(source, this.eventHandler);
        }
    }

    private void OnSourcePropertyChanged()
    {
        this.bindingControl.UpdateTargetValue();
    }
}