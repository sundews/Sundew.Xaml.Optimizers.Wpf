// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITwoWayBindingControl.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

internal interface ITwoWayBindingControl<TSourceValue, TTargetValue> : IBindingControl<TSourceValue, TTargetValue>
{
    TSourceValue GetSourceValue(TTargetValue targetValue);

    void UpdateSourceValue();
}