// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionaryCachingOptimizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Tests.Optimizations.ResourceDictionary;

using System.Xml.Linq;
using AwesomeAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.ResourceDictionary;
using Xunit;

public class ResourceDictionaryOptimizerTests
{
    private readonly XamlPlatformInfo xamlPlatformInfo;

    public ResourceDictionaryOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.SundewXamlOptimizationWpfNamespace);
    }

    [Fact]
    public void Optimize_When_ThereIsNothingToOptimize_Then_ResultShouldBeSameAsInput()
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
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([], true));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(input).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public void Optimize_When_ThereAreNestedMergedResourceDictionaries_Then_ResultShouldBeExpectedResult(string rootType)
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
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([], true));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public void Optimize_When_ThereIsOneMergedResourceDictionary_Then_ResultShouldBeExpectedResult(string rootType)
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
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([], true));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public void Optimize_When_ThereIsOneMergedUncategorizedResourceDictionaryAndReplaceUncategorizedIsNotSet_Then_ResultShouldBeExpectedResult(string rootType)
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
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([], false));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public void Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithReplaceCategory_Then_ResultShouldBeExpectedResult(string rootType)
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
    xmlns:local=""clr-namespace:Sundew.Xaml.Optimizers.Tests""
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
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([new OptimizationMapping("♻️", OptimizationAction.Replace, "local|clr-namespace:Sundew.Xaml.Optimizers.Tests|CustomResourceDictionary")], true));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("UserControl")]
    [InlineData("Page")]
    [InlineData("Window")]
    public void Optimize_When_ThereIsOneMergedResourceDictionaryMarkedWithRemoveCategory_Then_ResultShouldBeExpectedResult(string rootType)
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
                <!--<ResourceDictionary Source=""/Sundew.Xaml.Sample.Wpf;component/Controls.xaml""/> was commented out by ResourceDictionaryOptimizer-->
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var xDocument = XDocument.Parse(input);
        var testee = new ResourceDictionaryOptimizer(this.xamlPlatformInfo, new ResourceDictionarySettings([new OptimizationMapping("🎨", OptimizationAction.Remove)], true));

        var result = testee.Optimize(xDocument, Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }
}