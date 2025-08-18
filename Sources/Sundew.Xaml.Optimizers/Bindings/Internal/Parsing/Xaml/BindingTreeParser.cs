// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingTreeParser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class BindingTreeParser
{
    private const string DataContextAttributeName = "DataContext";
    private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;
    private readonly BindingMarkupExtensionParser bindingMarkupExtensionParser;
    private readonly bool optInToOptimizations;

    public BindingTreeParser(
        BindingXamlPlatformInfo bindingXamlPlatformInfo,
        BindingMarkupExtensionParser bindingMarkupExtensionParser,
        bool optInToOptimizations)
    {
        this.bindingXamlPlatformInfo = bindingXamlPlatformInfo;
        this.bindingMarkupExtensionParser = bindingMarkupExtensionParser;
        this.optInToOptimizations = optInToOptimizations;
    }

    public BindingTree? Parse(XElement xElement, string namespaceName, string baseTypeName, XamlTypeResolver xamlTypeResolver, XamlElementNameResolver xamlElementNameResolver)
    {
        if (!this.optInToOptimizations || this.IsOptimizing(xElement, false))
        {
            var xamlContext = new XamlContext(namespaceName, baseTypeName, xamlTypeResolver, this.bindingMarkupExtensionParser, xamlElementNameResolver);
            this.Traverse(xElement, null, new List<IBinding>(), xamlContext, true);
            return new BindingTree(xamlContext.BindingRootNodes);
        }

        return null;
    }

    private static string GetBindingRootName(string suggestedName, string typeName)
    {
        if (!string.IsNullOrEmpty(suggestedName))
        {
            var lastDotIndex = suggestedName.LastIndexOf('.');
            if (lastDotIndex > -1)
            {
                return suggestedName.Substring(lastDotIndex + 1);
            }

            return suggestedName;
        }

        return typeName;
    }

    private static string GetBindingRootType(string suggestedNamespaceQualifiedType, QualifiedType containerType)
    {
        if (!string.IsNullOrEmpty(suggestedNamespaceQualifiedType))
        {
            return suggestedNamespaceQualifiedType;
        }

        return containerType.ToNamespaceQualifiedType();
    }

    private void Traverse(XElement xElement, BindingRootContext? bindingRootContext, List<IBinding> bindings, XamlContext xamlContext, bool isOptimizing)
    {
        isOptimizing = this.IsOptimizing(xElement, isOptimizing);
        if (this.TryTraverseTypedTemplate(xElement, xamlContext, isOptimizing, this.bindingXamlPlatformInfo.DataTemplateDefinitions, DataTemplateCastDataContextBindingSourceNode.Create))
        {
            return;
        }

        if (this.TryTraverseTypedTemplate(xElement, xamlContext, isOptimizing, this.bindingXamlPlatformInfo.ControlTemplateDefinitions, ControlTemplateCastDataContextBindingSourceNode.Create))
        {
            return;
        }

        if (this.TryTraverseUntypedTemplate(xElement, xamlContext, isOptimizing, this.bindingXamlPlatformInfo.ItemsPanelTemplateDefinitions))
        {
            return;
        }

        if (this.TryTraverseUnsupportedElements(xElement, xamlContext, isOptimizing, this.bindingXamlPlatformInfo.UnsupportedElements))
        {
            return;
        }

        var elementName = xamlContext.XamlElementNameResolver.TryRegisterName(xElement);
        if (this.TryTraverseWithDataContextType(xElement, bindingRootContext, xamlContext, elementName, isOptimizing))
        {
            return;
        }

        this.TraverseElementBindingsAndChildren(xElement, bindingRootContext, bindings, xamlContext, elementName, isOptimizing);
    }

    private void TraverseElementBindingsAndChildren(XElement xElement, BindingRootContext? bindingRootContext, List<IBinding> bindings, XamlContext xamlContext, string elementName, bool isOptimizing)
    {
        var bindingId = 1;
        if (xElement.Attribute(DataContextAttributeName) is { } dataContextAttribute)
        {
            var result = xamlContext.BindingMarkupExtensionParser.Parse(dataContextAttribute);
            if (result.IsSuccess)
            {
                var dataContextBindings = new List<IBinding>();
                var dataContextTargetBindingNode = new DataContextTargetBindingNode(
                    xElement,
                    elementName,
                    result.Value,
                    result.Value.AdditionalValues.HasAny() ? bindingId++ : -1,
                    dataContextBindings,
                    isOptimizing);
                bindings.Add(dataContextTargetBindingNode);
                bindings = dataContextBindings;
            }
        }

        foreach (var xAttribute in xElement.Attributes().Where(x => x.Name.LocalName != DataContextAttributeName))
        {
            var bindingResult = xamlContext.BindingMarkupExtensionParser.Parse(xAttribute);
            if (bindingResult.IsSuccess)
            {
                var bindingMarkupExtension = bindingResult.Value;
                var sourceElementName = bindingMarkupExtension.ElementName;
                if (!string.IsNullOrEmpty(sourceElementName))
                {
                    var sourceElement = xamlContext.XamlElementNameResolver.Resolve(sourceElementName);
                    if (sourceElement == null)
                    {
                        throw new ElementNotFoundException(sourceElementName);
                    }

                    if (bindingRootContext.HasValue())
                    {
                        if (!bindingRootContext.ElementBindingSources.TryGetValue(sourceElementName, out var elementBindingPair))
                        {
                            var elementBindings = new List<IBinding>();
                            elementBindingPair = new ElementBindingPair(new ElementBindingSourceNode(sourceElement, sourceElementName, elementBindings), elementBindings);
                            bindingRootContext.ElementBindingSources.Add(sourceElementName, elementBindingPair);
                            bindings.Add(elementBindingPair.ElementBindingSourceNode);
                        }

                        elementBindingPair.Bindings.Add(new BindingNode(xElement, elementName, bindingResult.Value, bindingResult.Value.AdditionalValues.HasAny() ? bindingId++ : -1, isOptimizing));
                    }
                }
                else
                {
                    bindings.Add(new BindingNode(xElement, elementName, bindingResult.Value, bindingResult.Value.AdditionalValues.HasAny() ? bindingId++ : -1, isOptimizing));
                }
            }
        }

        this.TraverseChildren(xElement, bindingRootContext, bindings, xamlContext, isOptimizing);
    }

    private bool TryTraverseWithDataContextType(XElement xElement, BindingRootContext? bindingRootContext, XamlContext xamlContext, string elementName, bool isOptimizing)
    {
        var dataTypeAttribute = xElement.Attribute(this.bindingXamlPlatformInfo.DesignerDataContextName) ??
                                xElement.Attribute(this.bindingXamlPlatformInfo.SundewBindingsDataTypeName);

        if (dataTypeAttribute != null)
        {
            var castType = xamlContext.XamlTypeResolver.Parse(dataTypeAttribute.Value);
            if (castType.IsError)
            {
                return false;
            }

            var elementType = xamlContext.XamlTypeResolver.Parse(xElement.Name);
            if (elementType.IsError)
            {
                return false;
            }

            var xClassName = xElement.Attribute(this.bindingXamlPlatformInfo.XClassName)?.Value;
            if (!xClassName.HasValue())
            {
                return false;
            }

            var newBindingRootContext = new BindingRootContext((bindingRootContext?.HasCodeBehind).GetValueOrDefault(false) || !string.IsNullOrEmpty(xClassName));
            var castDataContextBindings = new List<IBinding>();
            var castDataContextSourceBinding = new CastDataContextBindingSourceNode(xElement, elementName, xClassName, castType.Value, castDataContextBindings);
            newBindingRootContext.Bindings.Add(castDataContextSourceBinding);
            xamlContext.BindingRootNodes.Add(
                new BindingRootNode(
                    xElement,
                    GetBindingRootType(xClassName, elementType.Value),
                    GetBindingRootName(xClassName, castType.Value.TypeName),
                    newBindingRootContext.Bindings,
                    newBindingRootContext.HasCodeBehind));
            this.TraverseElementBindingsAndChildren(xElement, newBindingRootContext, castDataContextBindings, xamlContext, elementName, isOptimizing);
            return true;
        }

        return false;
    }

    private bool TryTraverseUnsupportedElements(XElement xElement, XamlContext xamlContext, bool isOptimizing, IReadOnlyList<XName> unsupportedElements)
    {
        if (unsupportedElements.Contains(xElement.Name))
        {
            this.TraverseChildren(xElement, null, new List<IBinding>(), xamlContext, isOptimizing);
            return true;
        }

        return false;
    }

    private void TraverseChildren(
        XElement xElement,
        BindingRootContext? bindingRootContext,
        List<IBinding> bindings,
        XamlContext xamlContext,
        bool isOptimizing)
    {
        foreach (var childElement in xElement.Elements())
        {
            this.Traverse(childElement, bindingRootContext, bindings, xamlContext, isOptimizing);
        }
    }

    private bool TryTraverseTypedTemplate(XElement typedTemplateElement, XamlContext xamlContext, bool isOptimizing, IReadOnlyList<TypedTemplateDefinition> typedTemplateDefinitions, Func<XElement, string, string, XElement, QualifiedType, List<IBinding>, CastDataContextBindingSourceNode> nodeFactory)
    {
        var typedTemplateDefinition =
            typedTemplateDefinitions.FirstOrDefault(x =>
                x.FullName == typedTemplateElement.Name);
        if (typedTemplateDefinition != null)
        {
            var elementName = xamlContext.XamlElementNameResolver.TryRegisterName(typedTemplateElement);
            var contentElement =
                this.GetTemplateContentElement(typedTemplateElement, typedTemplateDefinition);
            if (contentElement != null)
            {
                var typeAttribute = typedTemplateElement.Attribute(typedTemplateDefinition.TypePropertyName);
                if (typeAttribute != null)
                {
                    var templateKey = typedTemplateElement.Attribute(this.bindingXamlPlatformInfo.XKeyName)?.Value;
                    if (templateKey == null)
                    {
                        return false;
                    }

                    var castType = xamlContext.XamlTypeResolver.Parse(typeAttribute.Value);
                    if (castType.IsError)
                    {
                        return false;
                    }

                    var contentElementTypeResult = xamlContext.XamlTypeResolver.Parse(contentElement.Name);
                    if (contentElementTypeResult.IsError)
                    {
                        return false;
                    }

                    var bindings = new List<IBinding>();
                    var bindingRootContext = new BindingRootContext(false);
                    bindingRootContext.Bindings.Add(
                        nodeFactory(
                            typedTemplateElement,
                            elementName,
                            templateKey,
                            contentElement,
                            castType.Value,
                            bindings));
                    xamlContext.BindingRootNodes.Add(
                        new BindingRootNode(
                            typedTemplateElement,
                            contentElementTypeResult.Value.ToNamespaceQualifiedType(),
                            GetBindingRootName(templateKey, castType.Value.TypeName),
                            bindingRootContext.Bindings,
                            bindingRootContext.HasCodeBehind));
                    this.TraverseChildren(contentElement, bindingRootContext, bindings, xamlContext, isOptimizing);
                }
                else
                {
                    this.TraverseChildren(contentElement, null, new List<IBinding>(), xamlContext, isOptimizing);
                }
            }

            return true;
        }

        return false;
    }

    private XElement? GetTemplateContentElement(XElement controlTemplateElement, ITemplateDefinition templateDefinition)
    {
        foreach (var xElement in controlTemplateElement.Elements())
        {
            var name = xElement.Name.ToString();
            if (!name.StartsWith($"{templateDefinition.FullName.LocalName}."))
            {
                return xElement;
            }

            if (name.Equals($"{templateDefinition.FullName.LocalName}.Template"))
            {
                return xElement.Elements().FirstOrDefault();
            }
        }

        return null;
    }

    private bool TryTraverseUntypedTemplate(
        XElement untypedTemplateElement,
        XamlContext xamlContext,
        bool isOptimizing,
        IReadOnlyList<UntypedTemplateDefinition> untypedTemplateDefinitions)
    {
        var untypedTemplateDefinition = untypedTemplateDefinitions.FirstOrDefault(x => x.FullName == untypedTemplateElement.Name);
        if (untypedTemplateDefinition != null)
        {
            var contentElement = this.GetTemplateContentElement(untypedTemplateElement, untypedTemplateDefinition);
            if (contentElement != null)
            {
                this.TraverseChildren(contentElement, null, new List<IBinding>(), xamlContext, isOptimizing);
            }

            return true;
        }

        return false;
    }

    private bool IsOptimizing(XElement xElement, bool isOptimizing)
    {
        var attribute = xElement.Attribute(this.bindingXamlPlatformInfo.SundewBindingsOptimizeBindingsName);
        if (attribute != null)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(attribute.Value, "true");
        }

        return isOptimizing;
    }
}