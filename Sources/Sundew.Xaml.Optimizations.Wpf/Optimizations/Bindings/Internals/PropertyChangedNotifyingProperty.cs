// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedNotifyingProperty.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internals;

using System;
using System.ComponentModel;
#if WINDOWS_UWP
    using Windows.UI.Xaml.Data;
#else
using System.Windows.Data;
#endif

internal class PropertyChangedNotifyingProperty<TSource> : INotifyingProperty<TSource>
{
    private readonly ISourceContext<TSource> sourceContext;
    private IBindingControl bindingControl;

    public PropertyChangedNotifyingProperty(ISourceContext<TSource> sourceContext, string name)
    {
        this.sourceContext = sourceContext;
        this.Name = name;
    }

    public string Name { get; }

    public void Initialize(IBindingControl bindingControl)
    {
        this.bindingControl = bindingControl;
    }

    public TSource TryUpdateSubscription(BindingMode bindingMode, TSource source)
    {
        if (DataBindingHelper.RequiresSourcePropertyChangeNotification(bindingMode))
        {
            if (this.Name == null)
            {
                return source;
            }

            var newSource = this.sourceContext.Source;
            if (!Equals(newSource, source))
            {
                this.Unsubscribe(source);
                source = newSource;
                if (!Equals(source, default))
                {
                    if (source is INotifyPropertyChanged notifyPropertyChanged)
                    {
#if WINDOWS_UWP
                            notifyPropertyChanged.PropertyChanged += this.OnSourcePropertyChanged;
#else
                        PropertyChangedEventManager.AddHandler(notifyPropertyChanged, this.OnSourcePropertyChanged, this.Name);
#endif
                    }
                }
            }
        }

        return source;
    }

    public void Unsubscribe(TSource source)
    {
        if (source is INotifyPropertyChanged oldNotifyPropertyChanged)
        {
#if WINDOWS_UWP
                oldNotifyPropertyChanged.PropertyChanged -= this.OnSourcePropertyChanged;
#else
            PropertyChangedEventManager.RemoveHandler(oldNotifyPropertyChanged, this.OnSourcePropertyChanged, this.Name);
#endif
        }
    }

    private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == this.Name || e.PropertyName == string.Empty)
        {
            this.sourceContext.Engine.Update(this.bindingControl);
            //// this.sourceContext.Engine.Update(this.SourcePropertyChanged);
            //// this.SourcePropertyChanged?.Invoke();
        }
    }
}