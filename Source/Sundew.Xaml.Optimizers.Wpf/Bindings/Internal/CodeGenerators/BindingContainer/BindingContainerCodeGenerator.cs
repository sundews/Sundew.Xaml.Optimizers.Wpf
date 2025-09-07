// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingContainerCodeGenerator.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingContainer;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Sundew.Base;
using Sundew.Base.Collections;
using Sundew.Base.Visiting;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeAnalysis;
using Sundew.Xaml.Optimizations.Bindings.Internal.CodeGenerators.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Extensions;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.Xaml;
using Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

internal class BindingContainerCodeGenerator : IBindingVisitor<Parameters, Context, bool, R<IReadOnlyList<GeneratedBindingContainer>>>
{
    private static readonly Version ToolVersion = Assembly.GetExecutingAssembly().GetName().Version;
    private readonly IBindingPathVisitor<BindingPath.Parameters, BindingPath.Context, R<BindingSource>, R<CodeInfo>> bindingPathCodeGenerator;
    private readonly BindingXamlPlatformInfo bindingXamlPlatformInfo;
    private readonly TypeResolver typeResolver;

    public BindingContainerCodeGenerator(IBindingPathVisitor<BindingPath.Parameters, BindingPath.Context, R<BindingSource>, R<CodeInfo>> bindingPathCodeGenerator, BindingXamlPlatformInfo bindingXamlPlatformInfo, TypeResolver typeResolver)
    {
        this.bindingPathCodeGenerator = bindingPathCodeGenerator;
        this.bindingXamlPlatformInfo = bindingXamlPlatformInfo;
        this.typeResolver = typeResolver;
    }

    public R<IReadOnlyList<GeneratedBindingContainer>> Visit(IBindingNode bindingNode, Parameters parameters, Context context)
    {
        var result = bindingNode.Visit(this, parameters, context);
        if (result)
        {
            var generatedContainers = new List<GeneratedBindingContainer>();
            foreach (var bindingContainerInfo in context.BindingContainerInfos)
            {
                var bindingContainerBaseType = this.typeResolver.GetType("Sundew.Xaml.Optimizations.BindingConnector`1");
                context.ExternAliases.TryAdd(bindingContainerBaseType);
                var sourceCode = $@"{GetNamespacesAndExternalAliases(this.bindingXamlPlatformInfo.DefaultUsingStatements, bindingContainerInfo.ExternalAliases)}

namespace {parameters.Namespace}
{{
    [global::System.CodeDom.Compiler.GeneratedCode(""Sundew.Xaml.Optimizations.Bindings"", ""{ToolVersion}"")]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class {bindingContainerInfo.BindingContainerType.TypeName} : {this.typeResolver.GetAliasQualifiedGenericType(bindingContainerBaseType, this.typeResolver.GetType(bindingContainerInfo.NamespaceQualifiedBindingRootType))}
    {{
        protected override void OnConnect()
        {{
{bindingContainerInfo.CodeBuilder}
        }}

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {{
            return new {bindingContainerInfo.BindingContainerType.TypeName}();
        }}
    }}
}}";
                generatedContainers.Add(new GeneratedBindingContainer(parameters.OutputPath, bindingContainerInfo.BindingContainerType, sourceCode));
            }

            return R.Success((IReadOnlyList<GeneratedBindingContainer>)generatedContainers);
        }

        return R.Error();
    }

    public void VisitUnknown(IBindingNode bindingNode, Parameters parameters, Context context)
    {
        throw VisitException.Create(bindingNode, parameters, context);
    }

    public bool BindingTree(BindingTree bindingTree, Parameters parameters, Context context)
    {
        var result = false;
        foreach (var bindingRootNode in bindingTree.BindingRoots)
        {
            result |= bindingRootNode.Visit(this, parameters, context);
        }

        return result;
    }

    public bool BindingRoot(BindingRootNode bindingRootNode, Parameters parameters, Context context)
    {
        if (bindingRootNode.Name != null)
        {
            var bindingContainerBuilder = new StringBuilder();
            var externalAliases = new HashSet<string>();
            if (this.VisitChildBindings(bindingRootNode.Bindings, parameters, new Context(bindingContainerBuilder, externalAliases, context, bindingRootNode.HasCodeBehind)))
            {
                context.BindingContainerInfos.Add(new BindingContainerInfo(parameters.BindingRootNodeTypes[bindingRootNode], bindingRootNode.NamespaceQualifiedType, bindingContainerBuilder, externalAliases));
                return true;
            }
        }

        return false;
    }

    public bool DataContextTargetBinding(
        DataContextTargetBindingNode dataContextTargetBindingNode,
        Parameters parameters,
        Context context)
    {
        var stringBuilder = new StringBuilder();
        var result = this.VisitDefiniteBinding(dataContextTargetBindingNode, parameters, context, stringBuilder);
        if (result.IsSuccess)
        {
            this.VisitChildBindings(dataContextTargetBindingNode.Bindings, parameters, new Context(stringBuilder, result.Value.BindingSource, context));
            context.BindingContainerSourceCodeBuilder.Append(stringBuilder);
            return true;
        }

        return false;
    }

    public bool CastDataContextBindingSource(
        CastDataContextBindingSourceNode castSourceBinding,
        Parameters parameters,
        Context context)
    {
        var sourceType = castSourceBinding.CastType;
        var bindingSource = context.BindingSourceProvider.AddDataContext(sourceType);
        var newContext = new Context(new StringBuilder(), bindingSource, context);
        if (this.VisitChildBindings(castSourceBinding.Bindings, parameters, newContext))
        {
            context.ExternAliases.TryAdd(sourceType);
            context.BindingContainerSourceCodeBuilder.AppendLine($"            var {bindingSource.Name} = this.GetDataContext(view => ({sourceType.ToAliasQualifiedType()})view.DataContext);");
            context.BindingContainerSourceCodeBuilder.Append(newContext.BindingContainerSourceCodeBuilder);
            return true;
        }

        return false;
    }

    public bool ControlTemplateCastDataContextBindingSource(
        ControlTemplateCastDataContextBindingSourceNode controlTemplateCastDataContextBindingSourceNode,
        Parameters parameters,
        Context context)
    {
        return false;
    }

    public bool DataTemplateCastDataContextBindingSource(
        DataTemplateCastDataContextBindingSourceNode dataTemplateCastDataContextBindingSourceNode,
        Parameters parameters,
        Context context)
    {
        var sourceType = dataTemplateCastDataContextBindingSourceNode.CastType;
        var bindingSource = context.BindingSourceProvider.AddDataContext(sourceType);
        var newContext = new Context(new StringBuilder(), bindingSource, context);
        if (this.VisitChildBindings(dataTemplateCastDataContextBindingSourceNode.Bindings, parameters, newContext))
        {
            context.ExternAliases.TryAdd(sourceType);
            context.BindingContainerSourceCodeBuilder.AppendLine($"            var {bindingSource.Name} = this.GetDataContext(view => ({sourceType.ToAliasQualifiedType()})view.DataContext);");
            context.BindingContainerSourceCodeBuilder.Append(newContext.BindingContainerSourceCodeBuilder);
            return true;
        }

        return false;
    }

    public bool ElementBindingSource(
        ElementBindingSourceNode elementBindingSourceNode,
        Parameters parameters,
        Context context)
    {
        var elementName = context.XamlElementNameProvider.GetName(elementBindingSourceNode);
        var elementType = parameters.XamlTypeResolver.Parse(elementBindingSourceNode.TargetElement.Name);
        if (elementType.IsError)
        {
            return false;
        }

        var bindingSource = context.BindingSourceProvider.AddElement(elementType.Value, elementName);
        var sourceVariableName = bindingSource.Name;
        var newContext = new Context(new StringBuilder(), bindingSource, context);
        if (this.VisitChildBindings(elementBindingSourceNode.Bindings, parameters, newContext))
        {
            context.ExternAliases.TryAdd(elementType.Value);
            context.BindingContainerSourceCodeBuilder.AppendLine($"            var {sourceVariableName} = this.GetElementContext({TargetCodeGenerator.GetTargetLambda(elementType.Value, elementName, context.HasCodeBehind)});");
            context.BindingContainerSourceCodeBuilder.Append(newContext.BindingContainerSourceCodeBuilder);
            return true;
        }

        return false;
    }

    public bool Binding(BindingNode bindingNode, Parameters parameters, Context context)
    {
        var result = this.VisitDefiniteBinding(bindingNode, parameters, context);
        if (result.IsSuccess)
        {
            context.BindingContainerSourceCodeBuilder.Append(result.Value.BindingPathSourceCodeBuilder);
            return true;
        }

        return false;
    }

    private static string GetNamespacesAndExternalAliases(string defaultUsingStatements, HashSet<string> externalAliases)
    {
        if (externalAliases.HasAny())
        {
            var stringBuilder = new StringBuilder();
            foreach (var externalAlias in externalAliases)
            {
                stringBuilder.AppendLine($"extern alias {externalAlias};");
            }

            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine(defaultUsingStatements);
            return stringBuilder.ToString();
        }

        return defaultUsingStatements;
    }

    private R<CodeInfo> VisitDefiniteBinding(IDefiniteBinding definiteBinding, Parameters parameters, Context context, StringBuilder? stringBuilder = null)
    {
        if (!definiteBinding.IsEnabled)
        {
            return R.Error();
        }

        return this.bindingPathCodeGenerator.Visit(
            definiteBinding.BindingAssignment.Path,
            new BindingPath.Parameters(parameters.XamlTypeResolver, definiteBinding, context.HasCodeBehind),
            new BindingPath.Context(stringBuilder ?? new StringBuilder(), context.BindingSource, context.XamlElementNameProvider, context.BindingSourceProvider, context.ExternAliases));
    }

    private bool VisitChildBindings(IReadOnlyList<IBinding> bindings, Parameters parameters, Context context)
    {
        var result = false;
        foreach (var binding in bindings)
        {
            result |= binding.Visit(this, parameters, context);
        }

        return result;
    }
}