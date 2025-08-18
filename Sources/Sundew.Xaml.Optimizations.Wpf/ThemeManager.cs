// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;

/// <summary>
/// Keeps track of the current applied theme and supports changing theme at runtime.
/// </summary>
/// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
public sealed class ThemeManager : IThemeManager
{
    private ThemeInfo? currentThemeInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeManager" /> class.
    /// </summary>
    /// <param name="themeInfos">The theme infos.</param>
    public ThemeManager(ObservableCollection<ThemeInfo> themeInfos)
    {
        this.ThemeInfos = themeInfos;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Occurs when the theme has changed.</summary>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// Gets the theme infos.
    /// </summary>
    /// <value>
    /// The theme infos.
    /// </value>
    public ObservableCollection<ThemeInfo> ThemeInfos { get; }

    /// <summary>
    /// Gets or sets the current theme.
    /// </summary>
    /// <value>
    /// The current theme.
    /// </value>
    public ThemeInfo? CurrentTheme
    {
        get => this.currentThemeInfo;
        set
        {
            if (value != null)
            {
                this.ChangeTheme(value);
            }
        }
    }

    /// <summary>
    /// Applies the specified theme.
    /// </summary>
    /// <param name="themeInfo">The theme information.</param>
    public void ChangeTheme(ThemeInfo themeInfo)
    {
        if (themeInfo == null)
        {
            throw new ArgumentNullException(nameof(themeInfo));
        }

        if (this.currentThemeInfo != null)
        {
            Application.Current.Resources.MergedDictionaries.Remove(this.currentThemeInfo.Theme);
        }

        if (themeInfo?.Theme != null)
        {
            var oldThemeInfo = this.currentThemeInfo;
            this.currentThemeInfo = themeInfo;
            Application.Current.Resources.MergedDictionaries.Add(this.currentThemeInfo.Theme);
            this.NotifyPropertyChanged(nameof(this.CurrentTheme));
            this.ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(oldThemeInfo, themeInfo));
        }
    }

    /// <summary>
    /// Notifies that the specified property has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}