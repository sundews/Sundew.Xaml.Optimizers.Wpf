// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryOptimizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Development.Tests.Optimizations.ResourceDictionary;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AwesomeAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.ResourceDictionary;
using Xunit;

public class ResourceDictionaryOptimizerTests
{
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ProjectInfo projectInfo;

    public ResourceDictionaryOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.WpfXamlNamespace);
        this.projectInfo = ProjectInfoHelper.ForTesting<ResourceDictionaryOptimizerTests>(false);
    }

    [Fact]
    public async Task Optimize_When_ThereIsNothingToOptimize_Then_ResultShouldBeSameAsInput()
    {
        var input = $@"<ResourceDictionary 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <ResourceDictionary.MergedDictionaries>
    </ResourceDictionary.MergedDictionaries>
    
    <Style TargetType=""{{x:Type ComboBox}}"">
        <Setter Property=""BorderThickness"" Value=""6""/>
    </Style>
</ResourceDictionary>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereAreNestedMergedResourceDictionaries_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    x:Class=""Sundew.Xaml.Sample.MainWindow""
    mc:Ignorable=""d"" Title=""MainWindow"" Height=""450"" Width=""800"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
    <Grid>
        <Grid.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls2.xaml"" />
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Grid.Resources>
    </Grid>
</{rootType}>";

        var expectedResult = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008"" 
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    x:Class=""Sundew.Xaml.Sample.MainWindow""
    mc:Ignorable=""d"" Title=""MainWindow"" Height=""450"" Width=""800"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <sx:ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
    <Grid>
        <Grid.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <sx:ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls2.xaml"" />
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Grid.Resources>
    </Grid>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionary_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:sx=""http://sundew.dev/xaml""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <sx:ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedUncategorizedResourceDictionaryAndReplaceUncategorizedIsNotSet_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([], false));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithReplaceCategory_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:sx=""http://sundew.dev/xaml""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" sx:ResourceDictionary.Category=""♻️"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:sx=""http://sundew.dev/xaml"" 
    xmlns:local=""clr-namespace:Sundew.Xaml.Optimizers.Tests;assembly=Sundew.Xaml.Optimizer.Tests""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <local:CustomResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([new OptimizationMapping("♻️", OptimizationAction.Replace, "local|clr-namespace:Sundew.Xaml.Optimizers.Tests;assembly=Sundew.Xaml.Optimizer.Tests|CustomResourceDictionary")], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithRemoveCategory_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:sx=""http://sundew.dev/xaml""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" sx:ResourceDictionary.Category=""🎨"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:sx=""http://sundew.dev/xaml"" 
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([new OptimizationMapping("🎨", OptimizationAction.Remove)], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithRemoveCategoryInDesignNamespace_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    mc:Ignorable=""d""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" d:ResourceDictionary.Category=""🎨"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    mc:Ignorable=""d"" 
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([new OptimizationMapping("🎨", OptimizationAction.Remove)], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithRemoveCategoryInSxDesignNamespace_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    xmlns:sxd=""http://sundew.dev/xaml/design""
    mc:Ignorable=""d sx""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" sxd:ResourceDictionary.Category=""🎨"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    xmlns:sxd=""http://sundew.dev/xaml/design""
    mc:Ignorable=""d sx""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([new OptimizationMapping("🎨", OptimizationAction.Remove)], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public async Task Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithRemoveCategoryWithSxInDesignNamespace_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    mc:Ignorable=""d""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml"" d:sx.ResourceDictionary.Category=""🎨"" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType} x:Class=""Sundew.Xaml.Optimizer.Sample""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    xmlns:sx=""http://sundew.dev/xaml""
    mc:Ignorable=""d""
    StartupUri=""MainWindow.xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(new ResourceDictionarySettings([new OptimizationMapping("🎨", OptimizationAction.Remove)], true));

        var result = await testee.OptimizeAsync([new XamlFile(xDocument, Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }
}