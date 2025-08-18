// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingOptimizationWriter.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal;

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Base.Text;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators;
using Sundew.Xaml.Optimizations.Bindings.Internal.XamlModification.BindingContainer;

internal class BindingOptimizationWriter
{
    private const string BindingsPrefix = "bindings";
    private readonly DirectoryInfo intermediateDirectory;
    private readonly XamlPlatformInfo xamlPlatformInfo;
    private readonly IFileSystem fileSystem;

    public BindingOptimizationWriter(DirectoryInfo intermediateDirectory, XamlPlatformInfo xamlPlatformInfo, IFileSystem fileSystem)
    {
        this.intermediateDirectory = intermediateDirectory;
        this.xamlPlatformInfo = xamlPlatformInfo;
        this.fileSystem = fileSystem;
    }

    public IReadOnlyList<AdditionalFile> ApplyOptimizations(IReadOnlyList<GeneratedBindingContainer> generatedBindingContainers)
    {
        var additionalFiles = new List<AdditionalFile>();
        foreach (var generatedBindingContainer in generatedBindingContainers)
        {
            var fileInfo = new FileInfo(Path.Combine(this.intermediateDirectory.FullName, generatedBindingContainer.OutputPath, generatedBindingContainer.BindingConnectorType.TypeName + ".cs"));
            this.fileSystem.Directory.CreateDirectory(fileInfo.DirectoryName);
            this.fileSystem.File.WriteAllText(fileInfo.FullName, generatedBindingContainer.SourceCode);
            additionalFiles.Add(new AdditionalFile(FileAction.Compile, fileInfo));
        }

        return additionalFiles;
    }

    public void ApplyOptimizations(IReadOnlyList<XamlModificationInfo> xamlModificationInfos, string sundewBindingsXamlNamespace)
    {
        foreach (var xamlModificationInfo in xamlModificationInfos)
        {
            foreach (var bindingXamlChange in xamlModificationInfo.BindingXamlChanges)
            {
                var stringBuilder = new StringBuilder();
                bindingXamlChange.Aggregate(
                    default(BindingXamlModification),
                    (previous, current) =>
                    {
                        previous?.EndApply(bindingXamlChange.TargetElement, stringBuilder);
                        current.BeginApply(bindingXamlChange.TargetElement, stringBuilder);
                        return current;
                    },
                    last =>
                    {
                        last?.EndApply(bindingXamlChange.TargetElement, stringBuilder);
                        return last;
                    });

                if (stringBuilder.Length > 0)
                {
                    bindingXamlChange.TargetElement.Add(new XAttribute(
                        XName.Get("BindingConnection.Metadata", sundewBindingsXamlNamespace),
                        stringBuilder.ToString()));
                }
            }

            xamlModificationInfo.TargetElement.Document?.Root?.EnsureXmlNamespaceAttribute(
                sundewBindingsXamlNamespace,
                BindingsPrefix,
                8,
                this.xamlPlatformInfo.PresentationNamespace,
                this.xamlPlatformInfo.XamlNamespace,
                this.xamlPlatformInfo.DesignerNamespace,
                this.xamlPlatformInfo.MarkupCompatibilityPrefix);

            var xmlNamespace = $"clr-namespace:{xamlModificationInfo.BindingConnectorType.NamespaceName}";
            var bindingConnectorXmlNamespace =
                xamlModificationInfo.TargetElement.Document?.Root?.EnsureXmlNamespaceAttribute(
                    xmlNamespace,
                    GetPrefix(xamlModificationInfo.BindingConnectorType.NamespaceName),
                    8,
                    this.xamlPlatformInfo.PresentationNamespace,
                    this.xamlPlatformInfo.XamlNamespace,
                    this.xamlPlatformInfo.DesignerNamespace,
                    this.xamlPlatformInfo.MarkupCompatibilityPrefix);
            if (bindingConnectorXmlNamespace.HasValue())
            {
                xamlModificationInfo.TargetElement.Add(new XAttribute(
                    XName.Get("BindingConnection.BindingConnector", sundewBindingsXamlNamespace),
                    $"{{{bindingConnectorXmlNamespace.Name.LocalName}:{xamlModificationInfo.BindingConnectorType.TypeName}}}"));
            }
        }
    }

    private static string GetPrefix(string namespaceName)
    {
        var index = namespaceName.LastIndexOf('.');
        if (index > -1)
        {
            return namespaceName.Substring(index + 1).Uncapitalize();
        }

        return namespaceName.Uncapitalize();
    }
}