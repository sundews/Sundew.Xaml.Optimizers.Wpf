// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlPlatformInfo.Uwp.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System;
using System.Collections.Generic;

internal partial class BindingXamlPlatformInfo
{
    private const string UpdateSourceTriggerTypeUwp = "Windows.UI.Xaml.Data.UpdateSourceTrigger";
    private const string BindingModeTypeUwp = "Windows.UI.Xaml.Data.BindingMode";

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> DefaultXamlTypeToSourceCodeNamespaceUwp =
        new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>>(() =>
            new Dictionary<string, IReadOnlyDictionary<string, Namespace>>());

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>>
        DefaultReadOnlyDependencyPropertyToNotificationEventUwp =
            new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>>(() =>
                new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>());
}