// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FreezeResourceOptimizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Tests.Optimizations.Freezing;

using System.Xml.Linq;
using AwesomeAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Freezing;
using Xunit;

public class FreezeResourceOptimizerTests
{
    private readonly XamlPlatformInfo xamlPlatformInfo;

    public FreezeResourceOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.SundewXamlOptimizationWpfNamespace);
    }

    [Fact]
    public void Optimize_When_DocumentContainsFrozenAndUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""BackgroundBrush"" po:Freeze=""False"" Color=""#111111"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""BackgroundBrush"" po:Freeze=""False"" Color=""#111111"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_DocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_PresentationOptionsNamespaceIsAlreadyReferencedAndDocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_PresentationOptionsNamespaceIsAlreadyReferencedButNotAddedToIgnorableAndDocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings(true));

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_IgnorableAlreadyExistsAndDocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_PresentationOptionsNamespaceIsAlreadyReferencedAndIgnorableAlreadyExistsAndDocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d po"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_PresentationOptionsNamespaceIsReferencedWithDifferentPrefixAndDocumentContainsUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:po1=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:po1=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d po1"">
    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po1:Freeze=""True"" />
    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po1:Freeze=""True"" />
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_ResourceDictionaryContainsNestedResourcesWithUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options"">
    <DataTemplate x:Key=""Key"">
        <Grid>
            <Grid.Resources>
                <ResourceDictionary>
                    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
                    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""False"" />
                </ResourceDictionary>
            </Grid.Resources>
        </Grid>
    </DataTemplate>
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <DataTemplate x:Key=""Key"">
        <Grid>
            <Grid.Resources>
                <ResourceDictionary>
                    <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
                    <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""False"" />
                </ResourceDictionary>
            </Grid.Resources>
        </Grid>
    </DataTemplate>
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_ResourceDictionaryContainsNestedResourceDictionaryWithUnfrozenResources_Then_ResultShouldBeExpectedResult()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options"">
    <DataTemplate x:Key=""Key"">
        <Grid>
            <Grid.Resources>
                <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
                <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""False"" />
            </Grid.Resources>
        </Grid>
    </DataTemplate>
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <DataTemplate x:Key=""Key"">
        <Grid>
            <Grid.Resources>
                <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
                <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""False"" />
            </Grid.Resources>
        </Grid>
    </DataTemplate>
</ResourceDictionary>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("Window")]
    [InlineData("Page")]
    [InlineData("UserControl")]
    public void Optimize_When_RootTypeContainsUnfrozenResources_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <{rootType}.Resources>
        <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
        <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <{rootType}.Resources>
        <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
        <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
    </{rootType}.Resources>
</{rootType}>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Application")]
    [InlineData("Window")]
    [InlineData("Page")]
    [InlineData("UserControl")]
    public void Optimize_When_RootTypeContainsUnfrozenResourcesNestedInResourceDictionary_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
            <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";

        var expectedResult = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <{rootType}.Resources>
        <ResourceDictionary>
            <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
            <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
        </ResourceDictionary>
    </{rootType}.Resources>
</{rootType}>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Window")]
    [InlineData("Page")]
    [InlineData("UserControl")]
    public void Optimize_When_RootTypeContainsNestedUnfrozenResourcesInResourceDictionary_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid.Resources>
            <ResourceDictionary>
                <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
                <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
            </ResourceDictionary>
        </Grid.Resources>
    </Grid>
</{rootType}>";

        var expectedResult = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <Grid>
        <Grid.Resources>
            <ResourceDictionary>
                <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
                <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
            </ResourceDictionary>
        </Grid.Resources>
    </Grid>
</{rootType}>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Theory]
    [InlineData("Window")]
    [InlineData("Page")]
    [InlineData("UserControl")]
    public void Optimize_When_RootTypeContainsNestedUnfrozenResources_Then_ResultShouldBeExpectedResult(string rootType)
    {
        var input = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Grid>
        <Grid.Resources>
            <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" />
            <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" />
        </Grid.Resources>
    </Grid>
</{rootType}>";

        var expectedResult = $@"<{rootType}
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:po=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/options""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""po"">
    <Grid>
        <Grid.Resources>
            <SolidColorBrush x:Key=""AccentBrush"" Color=""#AAAAAA"" po:Freeze=""True"" />
            <SolidColorBrush x:Key=""AccentBrush2"" Color=""#AAAAAA"" po:Freeze=""True"" />
        </Grid.Resources>
    </Grid>
</{rootType}>";
        var testee = new FreezeResourceOptimizer(this.xamlPlatformInfo, new FreezeResourceSettings());

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }
}