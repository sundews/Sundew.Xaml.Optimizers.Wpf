namespace Sundew.Xaml.Optimizers.Wpf.Tests.Optimizations.ThemeOptimizer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sundew.Base.Collections;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.Theme;
using Xunit;

public class ThemeOptimizerTests
{
    private readonly ProjectInfo projectInfo;
    private readonly XamlPlatformInfo xamlPlatformInfo;

    public ThemeOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.WpfXamlNamespace);
        this.projectInfo = ProjectInfoHelper.ForTesting<ThemeOptimizerTests>(false, projectDirectory: new FileInfo(typeof(ThemeOptimizerTests).Assembly.Location).Directory?.FullName ?? string.Empty);
    }
    [Fact]
    public async Task Test()
    {
        var testee = new ThemeOptimizer(new ThemeOptimizerSettings("Themes", "Modes"), this.xamlPlatformInfo, this.projectInfo);

        var result = await testee.OptimizeAsync(await this.GetXamlFiles());
    }

    private async Task<XamlFile[]> GetXamlFiles()
    {
        var files = new DirectoryInfo(Path.Combine(new FileInfo(typeof(ThemeOptimizerTests).Assembly.Location).Directory?.FullName ?? string.Empty, "Themes")).GetFiles("*.xaml", SearchOption.AllDirectories).Select(x => new FileReference(x.FullName, Path.GetRelativePath(this.projectInfo.ProjectDirectory.FullName, x.FullName)));
        return (await files.SelectAsync(async fileReference =>
        {
            var xDocument = XDocument.Parse(
                await File.ReadAllTextAsync(fileReference.Path),
                LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo | LoadOptions.SetBaseUri);
            return new XamlFile(
                xDocument,
                fileReference,
                GetLineEnding(xDocument));
        })).ToArray();
    }

    private const string WinNewLine = "\r\n";
    private const string UnixNewLine = "\n";

    /// <summary>
    /// Gets the line ending.
    /// </summary>
    /// <param name="xDocument">The x document.</param>
    /// <returns>The line ending.</returns>
    public static string GetLineEnding(XDocument xDocument)
    {
        var element = xDocument.Root;
        if (element != null)
        {
            var node = element.FirstNode;
            for (var i = 0; i < 10; i++)
            {
                if (node is XText xText)
                {
                    var text = xText.Value;
                    if (text != null)
                    {
                        if (text.Contains(WinNewLine))
                        {
                            return WinNewLine;
                        }

                        if (text.Contains(UnixNewLine))
                        {
                            return UnixNewLine;
                        }
                    }
                }

                node = element.Elements()?.FirstOrDefault()?.FirstNode;
            }
        }

        return Environment.NewLine;
    }

    private class FileReference : IFileReference
    {
        public FileReference(string path, string id)
        {
            this.Path = path;
            this.Id = id;
        }

        public string GetEvaluatedPath()
        {
            return this.Path;
        }

        public string Id { get; }

        public BuildAction BuildAction => BuildAction.Page;

        public string Path { get; }

        public IReadOnlyCollection<string> Names => [];

        public int Index => 0;

        public string this[string name] => string.Empty;

        public override string ToString()
        {
            return this.Path;
        }
    }
}
