// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal;

using System.IO;
using System.Linq;
using System.Text;
using Sundew.Xaml.Optimization;

internal readonly struct GeneratorInfo
{
    private const string LinkName = "Link";
    private const char DotCharacter = '.';
    private const char BackSlashCharacter = '\\';
    private const char SlashCharacter = '/';

    private GeneratorInfo(string outputPath, string containingAssemblyName, string @namespace)
    {
        this.OutputPath = outputPath;
        this.AssemblyName = containingAssemblyName;
        this.Namespace = @namespace;
    }

    public string OutputPath { get; }

    public string AssemblyName { get; }

    public string Namespace { get; }

    public static GeneratorInfo Get(IFileReference fileReference, string containingAssemblyName, string rootNamespace)
    {
        var id = fileReference.Id;
        var outputPath = Path.GetDirectoryName(id);
        if (fileReference.Names.Contains(LinkName))
        {
            var link = fileReference[LinkName];
            if (!string.IsNullOrEmpty(link))
            {
                outputPath = Path.GetDirectoryName(link);
            }
        }

        if (outputPath == null)
        {
            outputPath = string.Empty;
        }

        var namespaceBuilder = new StringBuilder(rootNamespace);
        if (!string.IsNullOrEmpty(outputPath))
        {
            namespaceBuilder.Append(DotCharacter);
            namespaceBuilder.Append(outputPath.Replace(BackSlashCharacter, DotCharacter).Replace(SlashCharacter, DotCharacter));
        }

        return new GeneratorInfo(outputPath, containingAssemblyName, namespaceBuilder.ToString());
    }
}