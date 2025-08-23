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
using Sundew.Xaml.Optimizers.StaticToDynamicResource;
using Xunit;

public class StaticToDynamicResourceOptimizerTests
{
    [Fact]
    public void Optimize_When_ResourceKeyContainsDynamicMarker_Then_StaticResourceShouldBeDynamicResource()
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
        <Setter Property=""Background"" Value=""{{DynamicResource DynamicBrush🔄️}}"" />
    </Style>
</ResourceDictionary>";
        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

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

        var testee = new StaticToDynamicResourceOptimizer(new StaticToDynamicResourceSettings("🔄️"));

        var result = testee.Optimize(XDocument.Parse(input), Substitute.For<IFileReference>());

        result.XDocument!.ToString().Should().Be(XDocument.Parse(input).ToString());
    }
}