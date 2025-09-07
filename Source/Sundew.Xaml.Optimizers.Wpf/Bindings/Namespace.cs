// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Namespace.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

/// <summary>Defines a namespace in source code.</summary>
public class Namespace
{
    /// <summary>Initializes a new instance of the <see cref="Namespace"/> class.</summary>
    /// <param name="name">Name of the namespace.</param>
    /// <param name="assemblyName">Name of the assembly.</param>
    public Namespace(string name, string assemblyName)
    {
        this.Name = name;
        this.AssemblyName = assemblyName;
    }

    /// <summary>Gets the name of the namespace.</summary>
    /// <value>The name of the namespace.</value>
    public string Name { get; }

    /// <summary>Gets the name of the assembly.</summary>
    /// <value>The name of the assembly.</value>
    public string AssemblyName { get; }
}