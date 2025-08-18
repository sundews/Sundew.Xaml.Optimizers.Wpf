// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml;

using System;
using System.IO;
using System.Windows;
using SystemResourceDictionary = System.Windows.ResourceDictionary;

/// <summary>
/// Contains a theme and it's name.
/// </summary>
public sealed class ThemeInfo
{
    private readonly Lazy<SystemResourceDictionary> theme;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeInfo"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="themeFactory">The theme factory.</param>
    public ThemeInfo(string name, Func<SystemResourceDictionary> themeFactory)
    {
        this.Name = name;
        this.theme = new Lazy<SystemResourceDictionary>(themeFactory);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeInfo"/> class.
    /// </summary>
    /// <param name="themeType">Type of the theme.</param>
    public ThemeInfo(Type themeType)
    {
        this.Name = themeType.Name;
        this.theme = new Lazy<SystemResourceDictionary>(() => (SystemResourceDictionary)Activator.CreateInstance(themeType)!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeInfo"/> class.
    /// </summary>
    /// <param name="uri">The URI.</param>
    public ThemeInfo(string uri)
    {
        this.Name = Path.GetFileNameWithoutExtension(uri);
        this.theme = new Lazy<SystemResourceDictionary>(() =>
            (SystemResourceDictionary)Application.LoadComponent(new Uri(uri, UriKind.RelativeOrAbsolute)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeInfo"/> class.
    /// </summary>
    /// <param name="uri">The URI.</param>
    public ThemeInfo(Uri uri)
    {
        this.Name = Path.GetFileNameWithoutExtension(uri.OriginalString);
        this.theme = new Lazy<SystemResourceDictionary>(() => (SystemResourceDictionary)Application.LoadComponent(uri));
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; }

    /// <summary>
    /// Gets the theme.
    /// </summary>
    /// <value>
    /// The theme.
    /// </value>
    public SystemResourceDictionary Theme => this.theme.Value;

    /// <summary>
    /// Gets a <see cref="ThemeInfo"/> from the specified type.
    /// </summary>
    /// <typeparam name="TTheme">The type of the theme.</typeparam>
    /// <returns>A new theme info.</returns>
    public static ThemeInfo FromType<TTheme>()
        where TTheme : SystemResourceDictionary
    {
        return new ThemeInfo(typeof(TTheme));
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return this.Name;
    }
}