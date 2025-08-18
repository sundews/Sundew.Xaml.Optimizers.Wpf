using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Xunit;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings
{
    public class BindingMarkupExtensionParserTests
    {
        private const string Empty = "";
        private const string Default = "Default";
        private readonly BindingMarkupExtensionParser testee;

        public BindingMarkupExtensionParserTests()
        {
            this.testee = new BindingMarkupExtensionParser(new BindingPathParser(new BindingPathLexicalAnalyzer()));
        }

        [Theory]
        [InlineData("Visibility", "{Binding Converter={local: NotNullToVisibilityConverter Invert = true}}", ".", Default, Empty, Empty, new[] { "Converter={local: NotNullToVisibilityConverter Invert = true}" })]
        [InlineData("ItemsSource", "{ Binding }", ".", Default, Empty, Empty, new string[0])]
        [InlineData("ItemsSource", "{Binding Todos}", "Todos", Default, Empty, Empty, new string[0])]
        [InlineData("ItemsSource", "{Binding Path=Todos, Converter={StaticResource ConverterName}}", "Todos", Default, Empty, Empty, new[] { "Converter={StaticResource ConverterName}" })]
        [InlineData("ItemsSource", "{Binding Path=Todos, Converter={StaticResource ConverterName}, ConverterParameter=en-US}", "Todos", Default, Empty, Empty, new[] { "Converter={StaticResource ConverterName}", "ConverterParameter=en-US" })]
        [InlineData("ItemsSource", "{Binding Path=Todos, Converter={diag:DebugConverter Converter={local:Converter }}}", "Todos", Default, Empty, Empty, new[] { "Converter={diag:DebugConverter Converter={local:Converter }}" })]
        [InlineData("ItemsSource", "{Binding Path=Count, Converter={diag:DebugConverter Converter={local:Converter }}, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target, UpdateSourceTrigger=LostFocus}", "Count", Default, Empty, "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("ItemsSource", "{Binding Path=Count, Converter={diag:DebugConverter Converter={local:Converter }}, ElementName=TextBlock, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target, UpdateSourceTrigger=LostFocus}", "Count", Default, "TextBlock", "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("ItemsSource", "{Binding Count, Converter={diag:DebugConverter Converter={local:Converter }}, ElementName=TextBlock, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target, UpdateSourceTrigger=LostFocus}", "Count", Default, "TextBlock", "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("ItemsSource", "{Binding Items.Count, Converter={diag:DebugConverter Converter={local:Converter }}, ElementName=TextBlock, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target, UpdateSourceTrigger=LostFocus}", "Items.Count", Default, "TextBlock", "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("ItemsSource", "{Binding ElementName=TextBlock, Path=Count, Converter={diag:DebugConverter Converter={local:Converter }}, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target, UpdateSourceTrigger=LostFocus}", "Count", Default, "TextBlock", "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("ItemsSource", "{Binding UpdateSourceTrigger=LostFocus, ElementName=TextBlock, Path=Count, Converter={diag:DebugConverter Converter={local:Converter }}, ConverterParameter=en-US, FallbackValue=0, TargetNullValue=No target}", "Count", Default, "TextBlock", "LostFocus", new[] { "Converter={diag:DebugConverter Converter={local:Converter }}", "ConverterParameter=en-US", "FallbackValue=0", "TargetNullValue=No target" })]
        [InlineData("Canvas.Top", "{Binding Height, Mode = OneWayToSource}", "Height", "OneWayToSource", Empty, Empty, new string[0])]
        public void Parse_Then_ResultsShouldBeExpectedResults(string targetProperty, string binding, string expectedPath, string expectedMode, string expectedElementName, string expectedUpdateSourceTrigger, string[] expectedAdditionalProperties)
        {
            var xAttribute = new XAttribute(targetProperty, binding);

            var result = testee.Parse(xAttribute);

            result.IsSuccess.Should().BeTrue();
            result.Value.TargetProperty.Should().Be(xAttribute);
            result.Value.Path.ToString().Should().Be(expectedPath);
            result.Value.Mode.ToString().Should().Be(expectedMode);
            result.Value.ElementName.Should().Be(expectedElementName);
            result.Value.UpdateSourceTrigger.Should().Be(expectedUpdateSourceTrigger);
            result.Value.AdditionalValues.Select(x => $"{x.Name}={x.Value}").Should().Equal(expectedAdditionalProperties);
        }
    }
}