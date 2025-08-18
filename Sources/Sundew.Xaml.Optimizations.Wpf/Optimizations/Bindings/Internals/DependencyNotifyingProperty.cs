// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyNotifyingProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

internal class DependencyNotifyingProperty<TSource> : INotifyingProperty<TSource>
{
    private readonly ISourceContext<TSource> sourceContext;
    private DependencyPropertyListener dependencyPropertyListener;
    private IBindingControl bindingControl;

    public DependencyNotifyingProperty(ISourceContext<TSource> sourceContext, DependencyProperty dependencyProperty)
    {
        this.sourceContext = sourceContext;
        this.DependencyProperty = dependencyProperty;
    }

    public DependencyProperty DependencyProperty { get; }

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
                    if (source is DependencyObject dependencyObject)
                    {
                        this.dependencyPropertyListener = DependencyPropertyListener.Subscribe(dependencyObject, this.DependencyProperty, this.OnDataContextInvalidateRequired);
                    }
                }
            }
        }

        return source;
    }

    public void Unsubscribe(TSource source)
    {
        this.dependencyPropertyListener?.Dispose();
        this.dependencyPropertyListener = null;
    }

    private void OnDataContextInvalidateRequired(object sender, EventArgs e)
    {
        this.bindingControl.UpdateTargetValue();
    }
}