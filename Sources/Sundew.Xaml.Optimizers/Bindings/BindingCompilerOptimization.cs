// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingCompilerOptimization.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
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

/// <summary>
/// Compiled bindings optimization.</summary>
/// <seealso cref="Sundew.Xaml.Optimization.IXamlOptimization" />
public class BindingCompilerOptimization : IXamlOptimization
{
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly ProjectInfo projectInfo;
    private readonly BindingTreeParser bindingTreeParser;
    private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;
    private readonly BindingOptimizationWriter bindingOptimizationWriter;
    private readonly BindingContainerCodeGenerator bindingContainerCodeGenerator;
    private readonly TypeResolver typeResolver;
    private readonly BindingContainerXamlModificationCollector bindingContainerXamlModificationCollector;

    /// <summary>Initializes a new instance of the <see cref="BindingCompilerOptimization"/> class.</summary>
    /// <param name="xamlPlatformInfo">The xaml platform information.</param>
    /// <param name="projectInfo">The project info.</param>
    /// <param name="bindingCompilerSettings">The binding compiler settings.</param>
    public BindingCompilerOptimization(XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo, BindingCompilerSettings bindingCompilerSettings)
        : this(xamlPlatformInfo, projectInfo, bindingCompilerSettings, new FileSystem())
    {
    }

    internal BindingCompilerOptimization(XamlPlatformInfo xamlPlatformInfo, ProjectInfo projectInfo, BindingCompilerSettings bindingCompilerSettings, IFileSystem fileSystem)
    {
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.projectInfo = projectInfo;
        this.bindingXamlPlatformInfo = new BindingXamlPlatformInfo(xamlPlatformInfo, bindingCompilerSettings);
        this.bindingTreeParser = new BindingTreeParser(
            this.bindingXamlPlatformInfo,
            new BindingMarkupExtensionParser(new BindingPathParser(new BindingPathLexicalAnalyzer())),
            bindingCompilerSettings.OptInToOptimizations);
        var codeAnalyzer = new CodeAnalyzer(
            this.projectInfo.AssemblyName,
            this.projectInfo.Compiles,
            this.projectInfo.AssemblyReferences,
            fileSystem.File,
            new XamlTypeBaseTypeSourceCodeGenerator(
                this.projectInfo.XDocumentProvider,
                this.bindingXamlPlatformInfo.XClassName,
                this.projectInfo.AssemblyName,
                this.projectInfo.AssemblyReferences,
                this.bindingXamlPlatformInfo.XamlTypeToSourceCodeNamespaces));
        this.typeResolver = new TypeResolver(codeAnalyzer);
        this.bindingOptimizationWriter = new BindingOptimizationWriter(this.projectInfo.IntermediateDirectory, xamlPlatformInfo, fileSystem);
        this.bindingContainerXamlModificationCollector = new BindingContainerXamlModificationCollector();
        this.bindingContainerCodeGenerator = new BindingContainerCodeGenerator(
            new BindingPathCodeGenerator(
                this.typeResolver,
                this.bindingXamlPlatformInfo,
                new ReadOnlyDependencyPropertyToNotificationEventResolver(codeAnalyzer, this.bindingXamlPlatformInfo.ReadOnlyDependencyPropertyNotificationEvents),
                new BindingModeResolver(this.bindingXamlPlatformInfo.OneWayBindingProperties, codeAnalyzer, xamlPlatformInfo.XamlPlatform),
                new TypeAssignmentCompatibilityAssessor(codeAnalyzer)),
            this.bindingXamlPlatformInfo,
            this.typeResolver);
    }

    /// <summary>Gets the supported platforms.</summary>
    /// <value>The supported platforms.</value>
    public IReadOnlyList<XamlPlatform> SupportedPlatforms => [XamlPlatform.WPF];

    /// <summary>Optimizes the specified xml document.</summary>
    /// <param name="xamlDocument">The xaml document.</param>
    /// <param name="xamlFile">The file info.</param>
    /// <returns>The result of the xaml optimization.</returns>
    public OptimizationResult Optimize(XDocument xamlDocument, IFileReference xamlFile)
    {
        var containingAssemblyName = this.projectInfo.AssemblyName;
        var xamlTypeResolver = XamlTypeResolver.FromXDocument(
            xamlDocument,
            containingAssemblyName,
            this.projectInfo.AssemblyReferences,
            this.bindingXamlPlatformInfo.XamlTypeToSourceCodeNamespaces);
        var xamlElementNameResolver = new XamlElementNameResolver(this.xamlPlatformInfo.XamlNamespace);
        var bindingTree = this.bindingTreeParser.Parse(
            xamlDocument.Root,
            Path.Combine(containingAssemblyName, Path.GetDirectoryName(xamlFile.Id)),
            Path.GetFileNameWithoutExtension(xamlFile.Id),
            xamlTypeResolver,
            xamlElementNameResolver);
        if (!bindingTree.HasValue())
        {
            return OptimizationResult.Error();
        }

        var generatorInfo = GeneratorInfo.Get(xamlFile, containingAssemblyName, this.projectInfo.RootNamespace);
        var xamlModificationCollectionResult = this.bindingContainerXamlModificationCollector.Visit(
            bindingTree,
            new Internal.XamlModification.BindingContainer.Parameters(generatorInfo, xamlTypeResolver),
            new Internal.XamlModification.BindingContainer.Context(new XamlElementNameProvider(xamlElementNameResolver), new XamlModificationTracker { ModificationsRootElement = xamlDocument.Root }));
        if (!xamlModificationCollectionResult.IsSuccess)
        {
            return OptimizationResult.Error();
        }

        var generatedContainers = this.bindingContainerCodeGenerator.Visit(
            bindingTree,
            new Internal.CodeGenerators.BindingContainer.Parameters(generatorInfo, xamlTypeResolver, xamlModificationCollectionResult.Value.BindingRootNodeTypes),
            new Internal.CodeGenerators.BindingContainer.Context(new XamlElementNameProvider(xamlElementNameResolver), new BindingSourceProvider(this.typeResolver)));

        if (generatedContainers.IsSuccess)
        {
            this.bindingOptimizationWriter.ApplyOptimizations(xamlModificationCollectionResult.Value.XamlModificationInfos, this.bindingXamlPlatformInfo.SundewBindingsXamlNamespace);
            var result = this.bindingOptimizationWriter.ApplyOptimizations(generatedContainers.Value);
            return OptimizationResult.Success(xamlDocument, result);
        }

        return OptimizationResult.Error();
    }
}