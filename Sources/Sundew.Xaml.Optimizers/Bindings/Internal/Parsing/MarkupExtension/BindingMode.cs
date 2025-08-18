// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingMode.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal enum BindingMode
{
    Default,
    OneWay,
    OneTime,
    OneWayToSource,
    TwoWay,
}