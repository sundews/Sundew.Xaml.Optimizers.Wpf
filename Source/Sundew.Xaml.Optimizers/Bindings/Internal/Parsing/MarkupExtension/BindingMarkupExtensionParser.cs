// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingMarkupExtensionParser.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.MarkupExtension;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sundew.Base;
using Sundew.Xaml.Optimizations.Bindings.Internal.Parsing.BindingPath;

internal class BindingMarkupExtensionParser
{
    private const string UnsupportedText = "Unsupported";
    private const string PathText = "Path";
    private const string ModeText = "Mode";
    private const string ElementNameText = "ElementName";
    private const string ConverterText = "Converter";
    private const string ConverterParameterText = "ConverterParameter";
    private const string FallbackValueText = "FallbackValue";
    private const string TargetNullValueText = "TargetNullValue";
    private const string UpdateSourceTriggerText = "UpdateSourceTrigger";
    private static readonly Regex BindingExtensionRegex = new Regex(@"^{(?: )*Binding(?: )+(?<Binding>(?:(?:(?:, +)?(?:Converter *= *(?<Converter>(((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?))?|((?:, +)(?:ConverterParameter *= *(?<ConverterParameter>(((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?))?|((?:, +)?(?:ElementName *= *(?<ElementName>((((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?)))?|((?:, +)?(?:Mode *= *(?<Mode>((((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?)))?|((?:, +)?(?:UpdateSourceTrigger *= *(?<UpdateSourceTrigger>((((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?)))?|((?:, +)(?:FallbackValue *= *(?<FallbackValue>(((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?))?|((?:, +)(?:TargetNullValue *= *(?<TargetNullValue>(((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?))?|((?:, +)?(?:(?:Path *= *)?(?<Path>(?:[\w\.\(\)])+)))|((?:, +)(?<Unsupported>((((?<cbopen>{)[^{]*)+([^{]*(?<-cbopen>}))+)+(?(cbopen)(?!))|[^,\n]+(?=, +)?)?))?)*)(?: )*}$");
    private readonly BindingPathParser bindingPathParser;

    public BindingMarkupExtensionParser(BindingPathParser bindingPathParser)
    {
        this.bindingPathParser = bindingPathParser;
    }

    public R<BindingAssignment> Parse(XAttribute xAttribute)
    {
        var input = xAttribute.Value;
        var match = BindingExtensionRegex.Match(input);
        if (!match.Success)
        {
            return R.Error();
        }

        if (match.Groups[UnsupportedText].Success)
        {
            return R.Error();
        }

        var pathResult = this.bindingPathParser.Parse(match.Groups[PathText].Value);
        if (!pathResult.IsSuccess)
        {
            return R.Error();
        }

        match.Groups[ModeText].Value.TryParseEnum(out BindingMode mode);
        var elementName = match.Groups[ElementNameText].Value;
        var updateSourceTrigger = match.Groups[UpdateSourceTriggerText].Value;

        var converterGroup = match.Groups[ConverterText];
        var converterParameterGroup = match.Groups[ConverterParameterText];
        var fallbackValueGroup = match.Groups[FallbackValueText];
        var targetNullValueGroup = match.Groups[TargetNullValueText];
        var list = new List<AdditionalValue>(4);
        TryAdd(ConverterText, converterGroup, list);
        TryAdd(ConverterParameterText, converterParameterGroup, list);
        TryAdd(FallbackValueText, fallbackValueGroup, list);
        TryAdd(TargetNullValueText, targetNullValueGroup, list);

        return R.Success(new BindingAssignment(xAttribute, pathResult.Value, mode, elementName, updateSourceTrigger, list));
    }

    private static void TryAdd(string name, Group group, List<AdditionalValue> list)
    {
        if (group.Success)
        {
            list.Add(new AdditionalValue(name, group.Value));
        }
    }
}