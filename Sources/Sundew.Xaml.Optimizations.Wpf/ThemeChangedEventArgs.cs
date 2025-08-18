// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeChangedEventArgs.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml;

using System;

/// <summary>Event args for the theme changed event.</summary>
public class ThemeChangedEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ThemeChangedEventArgs"/> class.</summary>
    /// <param name="oldThemeInfo">The old theme information.</param>
    /// <param name="newThemeInfo">The new theme information.</param>
    public ThemeChangedEventArgs(ThemeInfo? oldThemeInfo, ThemeInfo newThemeInfo)
    {
        this.OldThemeInfo = oldThemeInfo;
        this.NewThemeInfo = newThemeInfo;
    }

    /// <summary>Gets the old theme info.</summary>
    /// <value>The old theme information.</value>
    public ThemeInfo? OldThemeInfo { get; }

    /// <summary>Gets the new theme info.</summary>
    /// <value>The new theme information.</value>
    public ThemeInfo NewThemeInfo { get; }
}