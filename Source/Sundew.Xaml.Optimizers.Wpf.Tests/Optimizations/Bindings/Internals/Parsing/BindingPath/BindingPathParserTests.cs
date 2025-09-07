using FluentAssertions;
using Newtonsoft.Json;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.Visitors;
using Xunit;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings.Internals.Parsing.BindingPath
{
    public class BindingPathParserTests
    {
        private const string ExpectedName = @"{""Name"":""Name""}";
        private const string ExpectedPersonNameLength = @"{""Source"":{""Source"":{""Name"":""Person""},""Property"":{""Name"":""Name""}},""Property"":{""Name"":""Length""}}";
        private const string ExpectedDockPanelDock = @"{""XamlType"":{""NamespacePrefix"":"""",""TypeName"":""DockPanel""},""Name"":""Dock""}";
        private const string ExpectedDockPanelDockLength = @"{""Source"":{""XamlType"":{""NamespacePrefix"":"""",""TypeName"":""DockPanel""},""Name"":""Dock""},""Property"":{""Name"":""Length""}}";
        private const string ExpectedChildDockPanelDock = @"{""Source"":{""Name"":""Child""},""Property"":{""XamlType"":{""NamespacePrefix"":"""",""TypeName"":""DockPanel""},""Name"":""Dock""}}";
        private const string ExpectedPersonsIndexerInt6Length = @"{""Source"":{""Source"":{""Name"":""Persons""},""Indexer"":{""Literals"":[{""Type"":{""NamespacePrefix"":""sys"",""TypeName"":""Int32""},""Value"":""6""}]}},""Property"":{""Name"":""Length""}}";
        private const string ExpectedIndexerNameAge = @"{""Literals"":[{""Type"":null,""Value"":""Name""},{""Type"":null,""Value"":""Age""}]}";
        private const string ExpectedDot = @"{}";
        private const string ExpectedEmpty = @"{}";

        [Theory]
        [InlineData("Name", ExpectedName)]
        [InlineData("Person.Name.Length", ExpectedPersonNameLength)]
        [InlineData("(DockPanel.Dock)", ExpectedDockPanelDock)]
        [InlineData("(DockPanel.Dock).Length", ExpectedDockPanelDockLength)]
        [InlineData("Child.(DockPanel.Dock)", ExpectedChildDockPanelDock)]
        [InlineData("Persons[(sys:Int32)6].Length", ExpectedPersonsIndexerInt6Length)]
        [InlineData("[Name,Age]", ExpectedIndexerNameAge)]
        [InlineData("[Name, Age]", ExpectedIndexerNameAge)]
        [InlineData(".", ExpectedDot)]
        [InlineData("", ExpectedEmpty)]

        public void Parse_Then_ResultShouldBeExpectedResult(string input, string expectedResult)
        {
            var testee = new BindingPathParser(new BindingPathLexicalAnalyzer());

            var result = testee.Parse(input);

            result.IsSuccess.Should().BeTrue();
            JsonConvert.SerializeObject(result.Value).Should().Be(expectedResult);
        }


        [Theory]
        [InlineData("Name", null)]
        [InlineData("Person.Name.Length", null)]
        [InlineData("(DockPanel.Dock)", null)]
        [InlineData("(DockPanel.Dock).Length", null)]
        [InlineData("Child.(DockPanel.Dock)", null)]
        [InlineData("Persons[(sys:Int32)6].Length", null)]
        [InlineData("[Name,Age]", null)]
        [InlineData("[Name, Age]", "[Name,Age]")]
        [InlineData(".", null)]
        [InlineData("", ".")]

        public void Parse_When_ComparingWithToStringVisitor_Then_ResultShouldBeExpectedResult(string input, string expectedResult)
        {
            expectedResult ??= input;
            var testee = new BindingPathParser(new BindingPathLexicalAnalyzer());
            var toStringVisitor = new ToStringVisitor();

            var result = testee.Parse(input);

            result.IsSuccess.Should().BeTrue();
            toStringVisitor.Visit(result.Value, __._).Should().Be(expectedResult);
        }
    }
}