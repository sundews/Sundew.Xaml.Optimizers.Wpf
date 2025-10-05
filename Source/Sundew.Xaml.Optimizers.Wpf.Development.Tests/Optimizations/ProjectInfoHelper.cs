// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInfoHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizers.Wpf.Development.Tests.Optimizations;

using System.IO;
using System.Linq;
using NSubstitute;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;

public static class ProjectInfoHelper
{
    public static ProjectInfo ForTesting<TTestType>(bool isDebugging, IXamlFileProvider? xamlFileProvider = null, string? projectDirectory = null)
    {
        var testType = typeof(TTestType);
        return new ProjectInfo(
            testType.Assembly.GetName().Name ?? "TestAssembly",
            testType.Namespace,
            projectDirectory != null ? new DirectoryInfo(projectDirectory) : new FileInfo(testType.Assembly.Location).Directory?.Parent?.Parent ?? new DirectoryInfo(@"c:\temp"),
            new FileInfo(testType.Assembly.Location).Directory?.Parent?.Parent ?? new DirectoryInfo(@"c:\temp"),
            new DirectoryInfo(@"bin\obj\Debug"),
            testType.Assembly.GetReferencedAssemblies().Select(x => Substitute.For<IAssemblyReference>()).ToArray(),
            [],
            xamlFileProvider ?? Substitute.For<IXamlFileProvider>(),
            isDebugging);
    }
}