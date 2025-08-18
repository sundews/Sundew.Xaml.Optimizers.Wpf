// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlModificationInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

using System.Collections.Generic;
using System.Xml.Linq;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal class XamlModificationInfo
{
    public XamlModificationInfo(
        QualifiedType bindingConnectorType,
        XElement targetElement,
        IEnumerable<BindingXamlModifications> bindingXamlChanges)
    {
        this.BindingConnectorType = bindingConnectorType;
        this.TargetElement = targetElement;
        this.BindingXamlChanges = bindingXamlChanges;
    }

    public IEnumerable<BindingXamlModifications> BindingXamlChanges { get; }

    public QualifiedType BindingConnectorType { get; }

    public XElement TargetElement { get; }
}