// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXamlModification.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification;

using System.Text;
using System.Xml.Linq;

internal interface IXamlModification
{
    void BeginApply(XElement targetElement, StringBuilder stringBuilder);

    void EndApply(XElement targetElement, StringBuilder stringBuilder);
}