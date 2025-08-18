// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlModification.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using System.Text;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class BindingXamlModification : IXamlModification
{
    private readonly int bindingId;
    private readonly BindingAssignment bindingAssignment;
    private readonly XamlElementNameProvider xamlElementNameProvider;

    public BindingXamlModification(int bindingId, BindingAssignment bindingAssignment, XamlElementNameProvider xamlElementNameProvider)
    {
        this.bindingId = bindingId;
        this.bindingAssignment = bindingAssignment;
        this.xamlElementNameProvider = xamlElementNameProvider;
    }

    public void BeginApply(XElement targetElement, StringBuilder stringBuilder)
    {
        this.bindingAssignment.TargetProperty.Remove();
        var nameAttribute = this.xamlElementNameProvider.TryGetNameAttribute(targetElement);

        if (nameAttribute == null)
        {
            targetElement.Add(new XAttribute("Name", this.xamlElementNameProvider.GetName(targetElement)));
        }

        if (this.bindingId > -1)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append($"{{bindings:BindingData {this.bindingId}");
        }
    }

    public void EndApply(XElement targetElement, StringBuilder stringBuilder)
    {
        if (this.bindingId > -1)
        {
            this.GetValues(stringBuilder);
            stringBuilder.Append("}");
        }
    }

    private void GetValues(StringBuilder stringBuilder)
    {
        if (this.bindingId > -1)
        {
            foreach (var additionalValue in this.bindingAssignment.AdditionalValues)
            {
                stringBuilder.Append($", {additionalValue.Name}={additionalValue.Value}");
            }
        }
    }
}