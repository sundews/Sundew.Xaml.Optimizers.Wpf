// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticToDynamicResourceOptimizerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Tests.Optimizations.StaticToDynamicResource;

using System.Xml.Linq;
using AwesomeAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizers.StaticToDynamicResource;
using Xunit;

public class StaticToDynamicResourceOptimizerTests
{
    private readonly XamlPlatformInfo xamlPlatformInfo;

    public StaticToDynamicResourceOptimizerTests()
    {
        this.xamlPlatformInfo = new XamlPlatformInfo(XamlPlatform.WPF, Constants.WpfPresentationNamespace, Constants.WpfXamlNamespace);
    }

    [Fact]
    public void Optimize_When_ResourceKeyContainsDynamicMarkerAndIsNotInSameDictionary_Then_StaticResourceShouldBeDynamicResource()
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
        var testee = new StaticToDynamicResourceOptimizer(this.xamlPlatformInfo, new StaticToDynamicResourceSettings("🔄️"));

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_ResourceKeyContainsDynamicMarkerAndIsInSameDictionary_Then_StaticResourceShouldBeStaticResource()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""DynamicBrush🔄️"" Color=""Red"" />

    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";

        var expectedResult = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""DynamicBrush🔄️"" Color=""Red"" />

    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";
        var testee = new StaticToDynamicResourceOptimizer(this.xamlPlatformInfo, new StaticToDynamicResourceSettings("🔄️"));

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(expectedResult).ToString());
    }

    [Fact]
    public void Optimize_When_ResourceKeyDoesNotContainDynamicMarker_Then_StaticResourceShouldNotBeChanged()
    {
        var input = $@"<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <SolidColorBrush x:Key=""StaticBrush"" Color=""Red"" />
    <Style TargetType=""Button"">
        <Setter Property=""Background"" Value=""{{StaticResource StaticBrush}}"" />
    </Style>
</ResourceDictionary>";

        var testee = new StaticToDynamicResourceOptimizer(this.xamlPlatformInfo, new StaticToDynamicResourceSettings("🔄️"));

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(input).ToString());
    }
}