using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FluentAssertions;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimizations.Bindings;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;
using Xunit;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings.Internals.Parsing.Xaml
{
    extern alias sx;

    public class XamlTypeParserTests
    {
        private readonly XamlTypeResolver testee;

        public XamlTypeParserTests()
        {
            this.testee = new XamlTypeResolver("DefaultNameSpace", new List<XAttribute>() { new XAttribute(XNamespace.Xmlns + "todoDemo", "clr-namespace:Sundew.Xaml.Optimizations.ApiDesigner.Wpf.TodoDemo"), new XAttribute(XName.Get("xmlns", string.Empty), Constants.WpfPresentationNamespace) },
                new List<IAssemblyReference>(),
                new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>>(() =>
                    new Dictionary<string, IReadOnlyDictionary<string, Namespace>>()));
        }

        [Theory]
        [InlineData("{Binding Source={d:DesignInstance d:Type=todoDemo:ITodo}}", "Sundew.Xaml.Optimizations.ApiDesigner.Wpf.TodoDemo", "ITodo")]
        [InlineData("{d:DesignInstance d:Type=todoDemo:ITodo}", "Sundew.Xaml.Optimizations.ApiDesigner.Wpf.TodoDemo", "ITodo")]
        [InlineData("{x:Type todoDemo:ITodo}", "Sundew.Xaml.Optimizations.ApiDesigner.Wpf.TodoDemo", "ITodo")]
        [InlineData("todoDemo:ITodo", "Sundew.Xaml.Optimizations.ApiDesigner.Wpf.TodoDemo", "ITodo")]
        public void Parse_Then_ResultShouldBeExpectedResult(string input, string expectedNamespace, string expectedTypeName)
        {
            var result = this.testee.Parse(input);

            result.NamespaceName.Should().Be(expectedNamespace);
            result.TypeName.Should().Be(expectedTypeName);
        }
    }
}