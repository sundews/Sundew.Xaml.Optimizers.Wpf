// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssignmentCompatibility.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System;

[Flags]
internal enum TypeAssignmentCompatibility
{
    None,
    SourceToTarget = 1,
    TargetToSource = 2,
    BothWays = SourceToTarget | TargetToSource,
}