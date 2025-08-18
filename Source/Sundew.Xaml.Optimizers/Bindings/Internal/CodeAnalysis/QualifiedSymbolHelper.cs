// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedSymbolHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

internal static class QualifiedSymbolHelper
{
    public static string ToString(string assemblyAlias, string assemblyName, string namespaceName, string typeName)
    {
        return $"{typeName}|N:{namespaceName}|A:{assemblyName}|Alias:{assemblyAlias}";
    }

    public static string ToNamespaceQualifiedTypeString(string namespaceName, string typeName)
    {
        if (string.IsNullOrEmpty(namespaceName))
        {
            return typeName;
        }

        return $"{namespaceName}.{typeName}";
    }

    public static string ToAliasQualifiedTypeString(string assemblyAlias, string namespaceName, string typeName)
    {
        if (string.IsNullOrEmpty(namespaceName))
        {
            return typeName;
        }

        return $"{assemblyAlias}::{namespaceName}.{typeName}";
    }
}