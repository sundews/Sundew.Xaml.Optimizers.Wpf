// ----------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Sundew.Xaml;

[assembly: XmlnsDefinition(Constants.SundewXamlXmlNamespace, "Sundew.Xaml.Optimizations")]
[assembly: XmlnsPrefix(Constants.SundewXamlXmlNamespace, "sx")]
[assembly: InternalsVisibleTo("Sundew.Xaml.Optimizers.Tests")]