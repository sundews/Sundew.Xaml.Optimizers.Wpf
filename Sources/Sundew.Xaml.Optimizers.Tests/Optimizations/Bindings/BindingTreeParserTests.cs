extern alias sx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizations.Bindings;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;
using Xunit;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings
{
    public class BindingTreeParserTests
    {
        static BindingTreeParserTests()
        {
            WpfApplication.Initialize();
        }

        [Theory]
        [InlineData("Todos/ReflectionTodosControl.xaml", "Todos/ReflectionTodosControl.bter")]
        [InlineData("Bouncer/ReflectionBouncerControl.xaml", "Bouncer/ReflectionBouncerControl.bter")]
        public async Task Parse_Then_BindingTreeToStringVisitorShouldOutputExpectedResult(string inputFile, string expectedResultFile)
        {
            var xDocument = await LoadXDocument(inputFile).ConfigureAwait(false);
            var xamlPlatformInfo = XamlPlatformInfoProvider.GetXamlPlatformInfo(XamlPlatform.WPF);
            var bindingXamlPlatformInfo = new BindingXamlPlatformInfo(xamlPlatformInfo,
                new BindingCompilerSettings(
                    new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>(),
                    new Dictionary<string, IReadOnlyDictionary<string, Namespace>>(),
                    new Dictionary<string, IReadOnlyCollection<string>>()));
            var testee = new BindingTreeParser(
                bindingXamlPlatformInfo,
                new BindingMarkupExtensionParser(new BindingPathParser(new BindingPathLexicalAnalyzer())),
                false);

            var bindingTree = testee.Parse(
                xDocument.Root,
                Path.GetDirectoryName(inputFile),
                Path.GetFileNameWithoutExtension(inputFile),
                XamlTypeResolver.FromXDocument(xDocument, "Sundew.Xaml.Optimizations.TestData.ApiDesigner.Wpf", new IAssemblyReference[0], bindingXamlPlatformInfo.XamlTypeToSourceCodeNamespaces),
                new XamlElementNameResolver(xamlPlatformInfo.XamlNamespace));

            var visitor = new BindingTreeToStringVisitor();
            var result = visitor.Visit(bindingTree, null, 0);
            var expectedResult = GetExpectedResult(expectedResultFile);
            result.Should().Be(expectedResult);
        }

        private static string GetExpectedResult(string inputFile)
        {
            using var stringReader = new StreamReader(Application.GetResourceStream(GetUri(inputFile)).Stream);
            var expectedResult = stringReader.ReadToEnd();
            return expectedResult;
        }

        private static async Task<XDocument> LoadXDocument(string inputFile)
        {
            var xDocument = await XDocument.LoadAsync(Application.GetResourceStream(GetUri(inputFile)).Stream,
                LoadOptions.SetLineInfo | LoadOptions.SetBaseUri | LoadOptions.PreserveWhitespace,
                CancellationToken.None).ConfigureAwait(false);
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            XNamespace sundewXamlNamespace = "http://sundew.dev/xaml";
            XNamespace wpfNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            namespaceManager.AddNamespace(string.Empty, wpfNamespace.NamespaceName);
            namespaceManager.AddNamespace("sx", sundewXamlNamespace.NamespaceName);
            return xDocument;
        }

        private static Uri GetUri(string path)
        {
            return new Uri(
                Path.Combine("/Sundew.Xaml.UnitTests;component/Optimizations/Bindings/TestData/", path),
                UriKind.RelativeOrAbsolute);
        }
    }
}