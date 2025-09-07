// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeOptimizer.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Theme;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.ResourceDictionary.Internal;

/// <summary>
/// An optimizer that optimizes theme resources.
/// </summary>
public class ThemeOptimizer : IXamlOptimizer
{
    private const string XamlFilesPattern = "*.xaml";
    private const string DesignerXamlFileName = "Designer.xaml";
    private static readonly XamlType ThemeResourceDictionaryXamlType = new(Constants.SxPrefix, Constants.SundewXamlThemingNamespace, Constants.ThemeResourceDictionaryName);
    private static readonly XamlType ThemeModeResourceDictionaryXamlType = new(Constants.SxPrefix, Constants.SundewXamlThemingNamespace, Constants.ThemeModeResourceDictionaryName);
    private readonly ThemeOptimizerSettings themeOptimizerSettings;
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ProjectInfo projectInfo;
    private readonly DirectoryInfo themesDirectoryInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeOptimizer"/> class.
    /// </summary>
    /// <param name="themeOptimizerSettings">The theme optimizer settings.</param>
    /// <param name="xamlPlatformInfo">The xaml platform info.</param>
    /// <param name="projectInfo">The project info.</param>
    public ThemeOptimizer(ThemeOptimizerSettings themeOptimizerSettings, XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo)
    {
        this.themeOptimizerSettings = themeOptimizerSettings;
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.projectInfo = projectInfo;
        this.themesDirectoryInfo = new DirectoryInfo(Path.Combine(this.projectInfo.ProjectDirectory.FullName, this.themeOptimizerSettings.ThemesPath));
    }

    /// <inheritdoc/>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF];

    /// <inheritdoc/>
    public ValueTask<OptimizationResult> OptimizeAsync(IReadOnlyList<XamlFile> xamlFiles)
    {
        var themeDefinitionFiles = xamlFiles.Where(xamlFile => this.BelongsTo(xamlFile, this.themesDirectoryInfo, SearchOption.TopDirectoryOnly)).ToArray();
        var themes = themeDefinitionFiles.Select(xamlFile =>
        {
            var fileInfo = new FileInfo(xamlFile.Reference.Path);
            return new Theme(xamlFile, new DirectoryInfo(Path.Combine(fileInfo.Directory?.FullName ?? string.Empty, Path.GetFileNameWithoutExtension(fileInfo.Name))));
        }).Where(x => x.ThemeDirectoryInfo.Exists).ToArray();

        var themeDatas = themes
            .Select(theme =>
            {
                var themeModes = xamlFiles.Where(xamlFile => this.BelongsTo(
                        xamlFile,
                        new DirectoryInfo(Path.Combine(theme.ThemeDirectoryInfo.FullName, this.themeOptimizerSettings.ThemeModesPath)),
                        SearchOption.TopDirectoryOnly))
                    .Select(xamlFile =>
                    {
                        var fileInfo = new FileInfo(xamlFile.Reference.Path);
                        return new ThemeMode(xamlFile, new DirectoryInfo(Path.Combine(fileInfo.Directory?.FullName ?? string.Empty, Path.GetFileNameWithoutExtension(fileInfo.Name))));
                    })
                    .Where(x => x.ThemeModeDirectoryInfo.Exists)
                    .ToArray();
                var themeModesDirectoryInfo = new DirectoryInfo(Path.Combine(theme.ThemeDirectoryInfo.FullName, this.themeOptimizerSettings.ThemeModesPath));
                return new ThemeData(
                    theme,
                    themeModes.Select(resourceSetInfo => new ThemeModeData(
                                resourceSetInfo,
                                this.GetFiles(xamlFiles, resourceSetInfo.ThemeModeDirectoryInfo, null))).ToArray(),
                    this.GetFiles(xamlFiles, theme.ThemeDirectoryInfo, themeModesDirectoryInfo),
                    this.GetShared(xamlFiles, themeModesDirectoryInfo, themeModes));
            }).ToArray();

        var themeRoot = new ThemeRoot(themeDatas, this.GetShared(xamlFiles, this.themesDirectoryInfo, themes));

        var additionalFiles = new ConcurrentBag<AdditionalFile>();
        var xamlFileChanges = new ConcurrentBag<XamlFileChange>();
        foreach (var themeRootSharedThemeXamlFile in themeRoot.SharedThemeXamlFiles)
        {
            // Remove sharedXamlFile
            xamlFileChanges.Add(XamlFileChange.Remove(themeRootSharedThemeXamlFile));
        }

        foreach (var themeData in themeRoot.Themes)
        {
            // Create additional files
            this.UpdateResources(themeData.Theme, themeRoot.SharedThemeXamlFiles, themeData.ThemeFiles, additionalFiles, ThemeResourceDictionaryXamlType, xamlFileChanges);
            foreach (var sharedXamlFile in themeData.SharedModeXamlFiles)
            {
                // Remove sharedXamlFile
                xamlFileChanges.Add(XamlFileChange.Remove(sharedXamlFile));
            }

            foreach (var themeModeData in themeData.ThemeModes)
            {
                this.UpdateResources(themeModeData.ThemeMode, themeData.SharedModeXamlFiles, themeModeData.ThemeModeFiles, additionalFiles, ThemeModeResourceDictionaryXamlType, xamlFileChanges);
            }

            // Update themeData.Theme.MainXamlFile
            xamlFileChanges.Add(XamlFileChange.Update(themeData.Theme.MainXamlFile));
        }

        return OptimizationResult.From(xamlFileChanges, additionalFiles);
    }

    private void UpdateResources(Resource resource, IReadOnlyList<XamlFile> sharedResourceFiles, IReadOnlyList<XamlFile> resourceFiles, ConcurrentBag<AdditionalFile> additionalFiles, XamlType resourceDictionaryType, ConcurrentBag<XamlFileChange> xamlFileChanges)
    {
        // Create additional files
        var sharedThemeModeUris = sharedResourceFiles.Select(xamlFile => this.PrepareSharedFiles(xamlFile, resource.DirectoryInfo, resourceFiles, resourceDictionaryType, additionalFiles)).ToArray();

        // Add MergedRDs sharedXamlFile to themeModeData.ThemeMode.MainXamlFile ThemeModeResourceDictionary
        var wasChanged = this.AddResources(resource.MainXamlFile.Document, sharedThemeModeUris, resourceDictionaryType, resource.MainXamlFile.LineEndings);
        if (wasChanged)
        {
            // Update themeModeData.ThemeMode.MainXamlFile
            xamlFileChanges.Add(XamlFileChange.Update(resource.MainXamlFile));
        }
    }

    private bool AddResources(XDocument xDocument, Uri[] sharedResourceUris, XamlType resourceDictionaryXamlType, string lineEndings)
    {
        if (sharedResourceUris.Length == 0)
        {
            return false;
        }

        var mergedResourceDictionaryElement = xDocument.XPathSelectElement(
            Constants.DefaultResourceDictionaryMergedDictionariesXPath,
            this.xamlPlatformInfo.XmlNamespaceResolver);
        if (!mergedResourceDictionaryElement.HasValue())
        {
            mergedResourceDictionaryElement = new XElement(this.xamlPlatformInfo.SystemResourceDictionaryName + Constants.MergedDictionaries);
            var resourceDictionaryElement = xDocument.XPathSelectElement(
                Constants.DefaultResourceDictionaryXPath,
                this.xamlPlatformInfo.XmlNamespaceResolver);
            if (!resourceDictionaryElement.HasValue())
            {
                return false;
            }

            resourceDictionaryElement.AddFirst(new XText(lineEndings), new XText("    "), mergedResourceDictionaryElement);
        }

        foreach (var sharedResourceUri in sharedResourceUris)
        {
            var resourceDictionaryElement = new XElement(resourceDictionaryXamlType.Namespace + resourceDictionaryXamlType.Name);
            resourceDictionaryElement.Add(new XAttribute(Constants.SourceText, sharedResourceUri.OriginalString));
            if (mergedResourceDictionaryElement.PreviousNode is XText previousTextNode)
            {
                var replace = previousTextNode.Value.Replace(lineEndings, string.Empty);
                mergedResourceDictionaryElement.Add(new XText(lineEndings + replace + replace), resourceDictionaryElement);
            }
            else
            {
                mergedResourceDictionaryElement.Add(new XText(lineEndings + "        "), resourceDictionaryElement);
            }
        }

        mergedResourceDictionaryElement.LastNode.AddAfterSelf(new XText(lineEndings + "    "));

        return true;
    }

    private Uri PrepareSharedFiles(
        XamlFile sharedXamlFile,
        DirectoryInfo resourceDirectoryInfo,
        IReadOnlyList<XamlFile> resourceFiles,
        XamlType resourceDictionaryType,
        ConcurrentBag<AdditionalFile> additionalFiles)
    {
        var sharedThemeModeLinkPath = Path.Combine(Path.Combine(Path.GetDirectoryName(sharedXamlFile.Reference.Id) ?? string.Empty, resourceDirectoryInfo.Name), Path.GetFileName(sharedXamlFile.Reference.Path));

        var xDocument = new XDocument(sharedXamlFile.Document);
        this.AddResources(xDocument, resourceFiles.Select(x => this.GetXamlSourceUri(x.Reference.Id)).ToArray(), resourceDictionaryType, sharedXamlFile.LineEndings);

        // Output to intermediate file
        var fileInfo = new FileInfo(Path.Combine(this.projectInfo.IntermediateDirectory.FullName, sharedThemeModeLinkPath));
        additionalFiles.Add(new AdditionalFile(this.xamlPlatformInfo.DefaultItemType, fileInfo, xDocument.ToString(SaveOptions.DisableFormatting), sharedThemeModeLinkPath));
        return this.GetXamlSourceUri(sharedThemeModeLinkPath);
    }

    private Uri GetXamlSourceUri(string linkPath)
    {
        return new Uri($"/{this.projectInfo.AssemblyName};component/{linkPath.Replace('\\', '/')}", UriKind.Relative);
    }

    private IReadOnlyList<XamlFile> GetFiles(IReadOnlyList<XamlFile> xamlFiles, DirectoryInfo directoryInfo, DirectoryInfo? themeModesDirectoryInfo)
    {
        return xamlFiles.Where(xamlFile => this.BelongsTo(xamlFile, directoryInfo, SearchOption.AllDirectories) && (themeModesDirectoryInfo == null || !xamlFile.Reference.Path.StartsWith(themeModesDirectoryInfo.FullName))).ToArray();
    }

    private IReadOnlyList<XamlFile> GetShared(IReadOnlyList<XamlFile> xamlFiles, DirectoryInfo themeDirectory, Resource[] resourceSets)
    {
        return xamlFiles.Where(xamlFile => this.BelongsTo(xamlFile, themeDirectory, SearchOption.TopDirectoryOnly) && resourceSets.All(resourceSet => resourceSet.MainXamlFile != xamlFile)).ToArray();
    }

    private bool BelongsTo(XamlFile xamlFile, DirectoryInfo directoryInfo, SearchOption searchOption)
    {
        return directoryInfo.GetFiles(XamlFilesPattern, searchOption).Any(fileInfo => xamlFile.Reference.Path == fileInfo.FullName && fileInfo.Name != DesignerXamlFileName);
    }

    private abstract record Resource(XamlFile MainXamlFile, DirectoryInfo DirectoryInfo);

    private record ThemeRoot(IReadOnlyList<ThemeData> Themes, IReadOnlyList<XamlFile> SharedThemeXamlFiles);

    private record Theme(XamlFile MainXamlFile, DirectoryInfo ThemeDirectoryInfo) : Resource(MainXamlFile, ThemeDirectoryInfo);

    private record ThemeData(Theme Theme, IReadOnlyList<ThemeModeData> ThemeModes, IReadOnlyList<XamlFile> ThemeFiles, IReadOnlyList<XamlFile> SharedModeXamlFiles);

    private record ThemeMode(XamlFile MainXamlFile, DirectoryInfo ThemeModeDirectoryInfo) : Resource(MainXamlFile, ThemeModeDirectoryInfo);

    private record ThemeModeData(ThemeMode ThemeMode, IReadOnlyList<XamlFile> ThemeModeFiles);
}