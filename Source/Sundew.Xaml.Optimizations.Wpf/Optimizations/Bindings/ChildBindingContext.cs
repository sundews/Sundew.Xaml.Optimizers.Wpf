// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildBindingContext.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System;
using System.ComponentModel;
#if WINDOWS_UWP
    using Windows.UI.Xaml;
#else
using System.Windows;

#endif

/// <summary>A child data context.</summary>
/// <typeparam name="TRoot">The type of the root.</typeparam>
/// <typeparam name="TElement">The type of the element.</typeparam>
/// <typeparam name="TSource">The type of the source.</typeparam>
public class ChildBindingContext<TRoot, TElement, TSource> : BindingContext<TRoot, TSource>
    where TRoot : DependencyObject
{
    private TElement element;

    /// <summary>Initializes a new instance of the <see cref="ChildBindingContext{TRoot,TElement,TSource}"/> class.</summary>
    /// <param name="root">The root.</param>
    /// <param name="element">The element.</param>
    /// <param name="getSource">The get source.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="canSourceChange">The is data context.</param>
    internal ChildBindingContext(TRoot root, TElement element, Func<TElement, TSource> getSource, string propertyName, bool canSourceChange)
        : base(root, _ => getSource(element), canSourceChange)
    {
        this.element = element;
    }

    /// <summary>Occurs when the source has changed.</summary>
    protected override void OnSourceChanged(TSource oldSource)
    {
        base.OnSourceChanged(oldSource);
        this.UpdateElementDataContext(this.element);
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing">
    ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.element = default;
        }

        base.Dispose(disposing);
    }

    private void UpdateElementDataContext(TElement element)
    {
        var source = this.Source;
        switch (element)
        {
            case FrameworkElement frameworkElement:
                frameworkElement.DataContext = source;
                break;
#if WPF
            case FrameworkContentElement frameworkContentElement:
                frameworkContentElement.DataContext = source;
                break;
#endif
        }
    }
}