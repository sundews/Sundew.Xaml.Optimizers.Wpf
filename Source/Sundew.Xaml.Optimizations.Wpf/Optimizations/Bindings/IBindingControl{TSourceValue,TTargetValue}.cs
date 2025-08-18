// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBindingControl{TSourceValue,TTargetValue}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

internal interface IBindingControl<in TSourceValue, out TTargetValue> : IBindingControl
{
    TTargetValue Convert(TSourceValue sourceValue);
}