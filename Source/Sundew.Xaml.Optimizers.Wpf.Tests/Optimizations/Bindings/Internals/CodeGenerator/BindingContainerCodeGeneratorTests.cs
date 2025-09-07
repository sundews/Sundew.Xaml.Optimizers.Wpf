using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizations.Bindings;
using Sundew.Xaml.Optimizations.Bindings.Internal;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath.LexicalAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;
using Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;
using Xunit;
using Context = Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer.Context;
using Path = System.IO.Path;

namespace Sundew.Xaml.UnitTests.Optimizations.Bindings.Internals.CodeGenerator
{
    extern alias sx;

    public class BindingContainerCodeGeneratorTests
    {
        private const string ContainingAssemblyName = "Sundew.Xaml.Optimizations.TestData.Tester.Wpf";
        private const string RootNamespace = "Sundew.Xaml.Optimizations.TestData";
        private readonly XamlPlatformInfo xamlPlatformInfo;
        private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;
        private readonly IFileSystem fileSystem;
        private readonly BindingOptimizationWriter bindingOptimizationWriter;
        private readonly BindingContainerXamlModificationCollector bindingContainerXamlModificationCollector;

        public BindingContainerCodeGeneratorTests()
        {
            this.xamlPlatformInfo = XamlPlatformInfoProvider.GetXamlPlatformInfo(XamlPlatform.WPF);
            this.bindingXamlPlatformInfo = new BindingXamlPlatformInfo(this.xamlPlatformInfo,
                new BindingCompilerSettings(
                    new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>(),
                    new Dictionary<string, IReadOnlyDictionary<string, Namespace>>(),
                    new Dictionary<string, IReadOnlyCollection<string>>(),
                    false));
            this.fileSystem = Substitute.For<IFileSystem>();
            this.bindingContainerXamlModificationCollector = new BindingContainerXamlModificationCollector();
            this.bindingOptimizationWriter = new BindingOptimizationWriter(new DirectoryInfo(@"c:\temp\sxo"), xamlPlatformInfo, this.fileSystem);
            this.fileSystem.File.ReadAllText(Arg.Any<string>()).Returns(info => ReadResourceStreamToEnd(info.Arg<string>()));
        }

        [Theory]
        [InlineData("Todos/ReflectionTodosControl.xaml", new[] { @"Todos\ITodo.cs", @"Todos\ITodosViewModel.cs", @"Todos\ReflectionTodosControl.xaml.cs" },
                "Todos/ExpectedTodosControl.xaml", new[] { @"Todos\ExpectedTodosControlBindingConnector.cs", @"Todos\ExpectedTodoDataTemplateBindingConnector.cs" })]
        [InlineData("Bouncer/ReflectionBouncerControl.xaml", new[] { @"Bouncer\ActualSize.cs", @"Bouncer\AnimationViewModel.cs", @"Bouncer\ElementViewModel.cs", @"Bouncer\ReflectionBouncerControl.xaml.cs", @"Bouncer\TrackerViewModel.cs", },
                "Bouncer/ExpectedBouncerControl.xaml", new[] { @"Bouncer\ExpectedBouncerControlBindingConnector.cs" })]
        public async Task Visit_Then_BindingTreeToStringVisitorShouldOutputExpectedResult(string inputFilePath, string[] compiles, string expectedXamlDocumentPath, string[] expectedSourceCodePaths)
        {
            var xamlDocument = await LoadXDocument(inputFilePath).ConfigureAwait(false);
            var assemblyReferences = GetAssemblyReferences(xamlPlatformInfo.XamlPlatform);
            var codeAnalyzer = new CodeAnalyzer(ContainingAssemblyName, compiles.Select(x =>
                {
                    var compile = Substitute.For<IFileReference>();
                    compile.Path.Returns(x);
                    return compile;
                }).ToList(),
                assemblyReferences,
                this.fileSystem.File,
                null,
                false);
            var typeResolver = new TypeResolver(codeAnalyzer);
            var xamlTypeResolver = XamlTypeResolver.FromXDocument(xamlDocument, ContainingAssemblyName, assemblyReferences, bindingXamlPlatformInfo.XamlTypeToSourceCodeNamespaces);
            var xamlElementNameResolver = new XamlElementNameResolver(xamlPlatformInfo.XamlNamespace);
            var bindingTreeParser = new BindingTreeParser(
                bindingXamlPlatformInfo,
                new BindingMarkupExtensionParser(new BindingPathParser(new BindingPathLexicalAnalyzer())),
                false);
            var bindingTree = bindingTreeParser.Parse(xamlDocument.Root, Path.GetDirectoryName(inputFilePath), Path.GetFileNameWithoutExtension(inputFilePath), xamlTypeResolver, xamlElementNameResolver);

            var fileReference = Substitute.For<IFileReference>();
            fileReference.Id.Returns(inputFilePath);
            var generatorInfo = GeneratorInfo.Get(fileReference, ContainingAssemblyName, RootNamespace);
            var xamlModificationCollectionResult = this.bindingContainerXamlModificationCollector.Visit(
                bindingTree,
                new Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer.Parameters(generatorInfo, xamlTypeResolver),
                new Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer.Context(new XamlElementNameProvider(xamlElementNameResolver)));

            var testee = new BindingContainerCodeGenerator(
                new BindingPathCodeGenerator(
                    typeResolver,
                    this.bindingXamlPlatformInfo,
                    new ReadOnlyDependencyPropertyToNotificationEventResolver(
                        codeAnalyzer,
                        bindingXamlPlatformInfo.ReadOnlyDependencyPropertyNotificationEvents),
                    new BindingModeResolver(bindingXamlPlatformInfo.OneWayBindingProperties, codeAnalyzer, xamlPlatformInfo.XamlPlatform),
                    new TypeAssignmentCompatibilityAssessor(codeAnalyzer)),
                this.bindingXamlPlatformInfo, typeResolver);

            var result = testee.Visit(
                bindingTree,
                new Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer.Parameters(in generatorInfo, xamlTypeResolver, xamlModificationCollectionResult.Value.BindingRootNodeTypes),
                new Context(new XamlElementNameProvider(xamlElementNameResolver), new BindingSourceProvider(typeResolver)));

            this.bindingOptimizationWriter.ApplyOptimizations(result.Value);
            this.bindingOptimizationWriter.ApplyOptimizations(xamlModificationCollectionResult.Value.XamlModificationInfos, this.bindingXamlPlatformInfo.SundewBindingsXamlNamespace);

            var sourceCodePairs = expectedSourceCodePaths.Zip(result.Value, (path, container) => new { FileName = Path.GetFileNameWithoutExtension(path), ExpectedContent = ReadResourceStreamToEnd(path), container.SourceCode });
            foreach (var sourceCodePair in sourceCodePairs)
            {
                sourceCodePair.SourceCode.Should().Be(sourceCodePair.ExpectedContent);
                this.fileSystem.File.Received(1).WriteAllText(Arg.Any<string>(), sourceCodePair.ExpectedContent);
            }

            xamlDocument.ToString().Trim().Should().Be((await LoadXDocument(expectedXamlDocumentPath).ConfigureAwait(false)).ToString().Trim());
        }

        private IReadOnlyList<IAssemblyReference> GetAssemblyReferences(XamlPlatform xamlPlatform)
        {
            if (xamlPlatform == XamlPlatform.WPF)
            {
                return new[]
                {
                    typeof(int).Assembly.Location,
                    typeof(DependencyObject).Assembly.Location,
                    typeof(FrameworkElement).Assembly.Location,
                    typeof(ICommand).Assembly.Location,
                    typeof(sx::Sundew.Xaml.Optimizations.Bindings.BindingConnection).Assembly.Location,
            }.Select(
                    x =>
                    {
                        var assemblyReference = Substitute.For<IAssemblyReference>();
                        assemblyReference.Path.Returns(x);
                        return assemblyReference;
                    }).ToList();
            }

            throw new NotSupportedException("Currently only WPF is supported.");
        }

        private static async Task<XDocument> LoadXDocument(string inputFile)
        {
            var xDocument = await XDocument.LoadAsync(GetResourceStream(inputFile).Stream,
                LoadOptions.SetLineInfo | LoadOptions.SetBaseUri | LoadOptions.PreserveWhitespace,
                CancellationToken.None).ConfigureAwait(false);
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            XNamespace sundewXamlNamespace = "http://sundew.dev/xaml";
            XNamespace wpfNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            namespaceManager.AddNamespace(string.Empty, wpfNamespace.NamespaceName);
            namespaceManager.AddNamespace("sx", sundewXamlNamespace.NamespaceName);
            return xDocument;
        }

        private static Uri GetTesteeUri(string path)
        {
            return new Uri(
                Path.Combine("/Sundew.Xaml.UnitTests;component/Optimizations/Bindings/TestData/", path),
                UriKind.RelativeOrAbsolute);
        }

        private static StreamResourceInfo GetResourceStream(string path)
        {
            return Application.GetResourceStream(GetTesteeUri(path));
        }

        private static string ReadResourceStreamToEnd(string path)
        {
            using var streamReader = new StreamReader(GetResourceStream(path).Stream);
            return streamReader.ReadToEnd();
        }
    }
}