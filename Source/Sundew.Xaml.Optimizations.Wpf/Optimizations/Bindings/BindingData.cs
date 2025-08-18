// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingData.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

#if WINDOWS_UWP
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Markup;
#else
using System;
using System.Windows.Data;
using System.Windows.Markup;
#endif

/// <summary>Contains information about a compiled binding.</summary>
public class BindingData : MarkupExtension
{
    /// <summary>Initializes a new instance of the <see cref="BindingData"/> class.</summary>
    /// <param name="id">The identifier.</param>
    /// <param name="nextBindingData">The next binding data.</param>
    public BindingData(int id, BindingData nextBindingData)
    {
        this.Id = id;
        this.NextBindingData = nextBindingData;
    }

    /// <summary>Initializes a new instance of the <see cref="BindingData"/> class.</summary>
    /// <param name="id">The identifier.</param>
    public BindingData(int id)
        : this(id, null)
    {
    }

    /// <summary>Gets the identifier.</summary>
    /// <value>The identifier.</value>
    public int Id { get; }

    /// <summary>Gets or sets the converter.</summary>
    /// <value>The converter.</value>
    public IValueConverter Converter { get; set; }

    /// <summary>Gets or sets the converter paramter.</summary>
    /// <value>The converter paramter.</value>
    public object ConverterParameter { get; set; }

    /// <summary>Gets or sets the fallback value.</summary>
    /// <value>The fallback value.</value>
    public object FallbackValue { get; set; }

    /// <summary>Gets or sets the target null value.</summary>
    /// <value>The target null value.</value>
    public object TargetNullValue { get; set; }

    /// <summary>Gets or sets the update source trigger.</summary>
    /// <value>The update source trigger.</value>
    public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

    /// <summary>Gets the next binding data.</summary>
    /// <value>The next binding data.</value>
    public BindingData NextBindingData { get; }

    /// <summary>Tries the get by identifier.</summary>
    /// <param name="id">The identifier.</param>
    /// <param name="bindingData">The binding data.</param>
    /// <returns>A value indicating whether the binding data was found.</returns>
    public bool TryGetById(in int id, out BindingData bindingData)
    {
        if (this.Id == id)
        {
            bindingData = this;
            return true;
        }

        bindingData = null;
        return this.NextBindingData?.TryGetById(id, out bindingData) == true;
    }

#if WINDOWS_UWP
        /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.</summary>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        protected override object ProvideValue()
        {
            return this;
        }
#else
    /// <summary>When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.</summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The object value to set on the property where the extension is applied.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
#endif
}