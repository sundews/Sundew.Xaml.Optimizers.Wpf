// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingAssignment.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

internal class BindingAssignment
{
    public BindingAssignment(XAttribute targetProperty, IBindingPathExpression path, BindingMode mode, string elementName, string updateSourceTrigger, IReadOnlyList<AdditionalValue> additionalValues)
    {
        this.TargetProperty = targetProperty;
        this.Path = path;
        this.Mode = mode;
        this.ElementName = elementName;
        this.UpdateSourceTrigger = updateSourceTrigger;
        this.AdditionalValues = additionalValues;
    }

    public XAttribute TargetProperty { get; }

    public IBindingPathExpression Path { get; }

    public BindingMode Mode { get; }

    public string ElementName { get; }

    public string UpdateSourceTrigger { get; }

    public IReadOnlyList<AdditionalValue> AdditionalValues { get; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(this.TargetProperty.Name.LocalName);
        stringBuilder.Append("=");
        stringBuilder.Append(this.Path);
        if (!string.IsNullOrEmpty(this.ElementName))
        {
            stringBuilder.Append(", ElementName=");
            stringBuilder.Append(this.ElementName);
        }

        if (!string.IsNullOrEmpty(this.UpdateSourceTrigger))
        {
            stringBuilder.Append(", UpdateSourceTrigger=");
            stringBuilder.Append(this.UpdateSourceTrigger);
        }

        foreach (var additionalProperty in this.AdditionalValues)
        {
            stringBuilder.Append(", ");
            stringBuilder.Append(additionalProperty.Name);
            stringBuilder.Append("=");
            stringBuilder.Append(additionalProperty.Value);
        }

        return stringBuilder.ToString();
    }
}