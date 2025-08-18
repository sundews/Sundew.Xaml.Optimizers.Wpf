// ----------------------------------------------------------------------------------------
// <copyright file="IThemeManager.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------------------------

namespace Sundew.Xaml;

using System.ComponentModel;

/// <summary>
/// Interface for implementing a theme manager.
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public interface IThemeManager : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    /// <value>
    /// The current theme.
    /// </value>
    ThemeInfo? CurrentTheme { get; set; }

    /// <summary>
    /// Applies the specified theme.
    /// </summary>
    /// <param name="themeInfo">The theme information.</param>
    void ChangeTheme(ThemeInfo themeInfo);
}