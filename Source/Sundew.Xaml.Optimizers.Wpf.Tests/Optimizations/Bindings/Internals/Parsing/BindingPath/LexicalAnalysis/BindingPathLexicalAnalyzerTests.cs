using System.Linq;
using FluentAssertions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Xunit;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings.Internals.Parsing.BindingPath.LexicalAnalysis
{
    public class BindingPathLexicalAnalyzerTests
    {
        [Theory]
        [InlineData("Name", new[] { "Name", "" })]
        [InlineData("Person.Name.Length", new[] { "Person", ".", "Name", ".", "Length", "" })]
        [InlineData("(DockPanel.Dock)", new[] { "(", "DockPanel", ".", "Dock", ")", "" })]
        [InlineData("(DockPanel.Dock).Length", new[] { "(", "DockPanel", ".", "Dock", ")", ".", "Length", "" })]
        [InlineData("Child.(DockPanel.Dock)", new[] { "Child", ".", "(", "DockPanel", ".", "Dock", ")", "" })]
        [InlineData("Persons[(sys:Int32)6].Length", new[] { "Persons", "[", "(", "sys", ":", "Int32", ")", "6", "]", ".", "Length", "" })]
        [InlineData("[Name,Age]", new[] { "[", "Name", ",", "Age", "]", "" })]
        [InlineData(".", new[] { ".", "" })]
        [InlineData("", new[] { "" })]
        public void Analyze_Then_ResultShouldBeExpectedResult(string input, string[] expectedLexemes)
        {
            var testee = new BindingPathLexicalAnalyzer();

            var result = testee.Analyze(input);

            result.IsSuccess.Should().BeTrue();
            result.Value.Select(x => x.Token).Should().Equal(expectedLexemes);
        }
    }
}








