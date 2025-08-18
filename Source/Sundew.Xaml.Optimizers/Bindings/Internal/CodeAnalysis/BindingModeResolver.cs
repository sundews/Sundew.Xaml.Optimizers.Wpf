// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingModeResolver.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

internal class BindingModeResolver
{
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> defaultOneWayBindingProperties;
    private readonly CodeAnalyzer codeAnalyzer;
    private readonly XamlPlatform xamlPlatform;

    public BindingModeResolver(
        Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> defaultOneWayBindingProperties,
        CodeAnalyzer codeAnalyzer,
        XamlPlatform xamlPlatform)
    {
        this.defaultOneWayBindingProperties = defaultOneWayBindingProperties;
        this.codeAnalyzer = codeAnalyzer;
        this.xamlPlatform = xamlPlatform;
    }

    public BindingMode GetBindingMode(BindingMode bindingMode, TargetValueCodeGenerator targetValueCodeGenerator, IAccessorCodeGenerator accessorCodeGenerator)
    {
        if (bindingMode == BindingMode.Default)
        {
            if (this.IsBindingOneWayPerDefault(targetValueCodeGenerator))
            {
                bindingMode = BindingMode.OneWay;
            }
            else
            {
                bindingMode = this.xamlPlatform == XamlPlatform.WPF ? BindingMode.TwoWay : BindingMode.OneWay;
            }
        }

        var sourceQualifiedProperty = accessorCodeGenerator.Accessor;
        switch (bindingMode)
        {
            case BindingMode.OneWay when !sourceQualifiedProperty.HasGetter:
                throw new NotSupportedException($"The source property {accessorCodeGenerator.Accessor.Type.ToNamespaceQualifiedType()}{sourceQualifiedProperty.Name} must have a getter");
            case BindingMode.OneTime when !sourceQualifiedProperty.HasGetter:
                throw new NotSupportedException($"The source property {sourceQualifiedProperty.Name} must have a getter");
            case BindingMode.OneWayToSource when !sourceQualifiedProperty.HasSetter:
                throw new NotSupportedException($"The source property {sourceQualifiedProperty.Name} must have a setter");
            case BindingMode.TwoWay when !sourceQualifiedProperty.HasGetter && !sourceQualifiedProperty.HasSetter:
                throw new NotSupportedException($"The source property {sourceQualifiedProperty.Name} must have a getter and/or setter");
            case BindingMode.TwoWay when sourceQualifiedProperty.HasGetter && !sourceQualifiedProperty.HasSetter:
                return BindingMode.OneWay;
            case BindingMode.TwoWay when !sourceQualifiedProperty.HasGetter && sourceQualifiedProperty.HasSetter:
                return BindingMode.OneWayToSource;
        }

        return bindingMode;
    }

    private bool IsBindingOneWayPerDefault(TargetValueCodeGenerator targetValueCodeGenerator)
    {
        var typeSymbol = this.codeAnalyzer.GetTypeSymbol(targetValueCodeGenerator.TargetType);
        while (typeSymbol != null)
        {
            if (this.defaultOneWayBindingProperties.Value.TryGetValue($"{typeSymbol.ContainingAssembly.Name}|{typeSymbol.ContainingNamespace}.{typeSymbol.Name}", out var assemblyType))
            {
                if (assemblyType.Contains(targetValueCodeGenerator.TargetProperty.Name))
                {
                    return true;
                }
            }

            typeSymbol = typeSymbol.BaseType;
        }

        return false;
    }
}