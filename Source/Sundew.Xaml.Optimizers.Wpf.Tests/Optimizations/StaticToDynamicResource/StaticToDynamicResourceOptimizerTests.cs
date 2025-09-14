﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticToDynamicResourceOptimizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Tests.Optimizations.StaticToDynamicResource;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AwesomeAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.Wpf.StaticToDynamicResource;
using Xunit;

public class StaticToDynamicResourceOptimizerTests
{
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ProjectInfo projectInfo;

    public StaticToDynamicResourceOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.WpfXamlNamespace);
        this.projectInfo = ProjectInfoHelper.ForTesting<StaticToDynamicResourceOptimizerTests>(false);
    }

    [Fact]
    public async Task Optimize_When_ResourceKeyContainsDynamicMarkerAndIsNotInSameDictionary_Then_StaticResourceShouldBeDynamicResource()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{DynamicResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";
        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

        var result = await testee.OptimizeAsync([new XamlFile(XDocument.Parse(input), Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Single().File.Document.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public async Task Optimize_When_ResourceKeyContainsDynamicMarkerAndIsInSameDictionary_Then_StaticResourceShouldBeStaticResource()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""DynamicBrush🔄️"" Color=""Red"" />

    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";

        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

        var result = await testee.OptimizeAsync([new XamlFile(XDocument.Parse(input), Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Should().BeEmpty();
    }

    [Fact]
    public async Task Optimize_When_ResourceKeyContainsDynamicMarkerAndIsWithinStoryBoard_Then_StaticResourceShouldBeStaticResource()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <Storyboard x:Key=""BackgroundAnimation🔄️"" AutoReverse=""True"" RepeatBehavior=""Forever"" >
        <ColorAnimationUsingKeyFrames Storyboard.TargetProperty=""(Control.Background).(GradientBrush.GradientStops)[0].(GradientStop.Color)"">
            <LinearColorKeyFrame KeyTime=""0:0:2"" Value=""{{StaticResource DynamicBrush🔄️}}""/>
        </ColorAnimationUsingKeyFrames>
    </Storyboard>

</ResourceDictionary>";

        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

        var result = await testee.OptimizeAsync([new XamlFile(XDocument.Parse(input), Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Should().BeEmpty();
    }

    [Fact]
    public async Task Optimize_When_ResourceKeyDoesNotContainDynamicMarker_Then_StaticResourceShouldNotBeChanged()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""StaticBrush"" Color=""Red"" />
    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource StaticBrush}}"" />
    </Style>
</ResourceDictionary>";

        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

        var result = await testee.OptimizeAsync([new XamlFile(XDocument.Parse(input), Substitute.For<IFileReference>(), Environment.NewLine)], this.xamlPlatformInfo, this.projectInfo);

        result.XamlFileChanges.Should().BeEmpty();
    }
}