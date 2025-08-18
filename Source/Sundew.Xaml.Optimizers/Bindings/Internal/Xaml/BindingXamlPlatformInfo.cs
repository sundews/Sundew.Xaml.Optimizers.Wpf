// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlPlatformInfo.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sundew.Xaml.Optimization;
using Sundew.Xaml.Optimization.Xml;

internal partial class BindingXamlPlatformInfo
{
    private static readonly Dictionary<XamlPlatform, string> StandardNamespacesDictionary = new Dictionary<XamlPlatform, string>();

    static BindingXamlPlatformInfo()
    {
        StandardNamespacesDictionary[XamlPlatform.WPF] =
            $@"using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;";
        StandardNamespacesDictionary[XamlPlatform.UWP] = string.Empty;
    }

    public BindingXamlPlatformInfo(XamlPlatformInfo xamlPlatformInfo, BindingCompilerSettings bindingCompilerSettings)
    {
        this.XamlPlatformInfo = xamlPlatformInfo;
        this.DataTriggerName = xamlPlatformInfo.PresentationNamespace + "DataTrigger";
        this.SetterName = xamlPlatformInfo.PresentationNamespace + "Setter";
        this.ApplicationName = xamlPlatformInfo.PresentationNamespace + "Application";
        this.SundewXamlNamespace = "http://sundew.dev/xaml";
        this.DesignerDataContextName = xamlPlatformInfo.DesignerNamespace + "DataContext";
        this.SundewBindingsDataTypeName = this.SundewXamlNamespace + "Bindings.DataType";
        this.SundewBindingsOptimizeBindingsName = this.SundewXamlNamespace + "Bindings.OptimizeBindings";
        this.XClassName = xamlPlatformInfo.XamlNamespace + "Class";
        this.XKeyName = xamlPlatformInfo.XamlNamespace + "Key";
        this.DataTemplateDefinitions = new List<TypedTemplateDefinition>
        {
            new TypedTemplateDefinition(xamlPlatformInfo.PresentationNamespace + "DataTemplate", "DataType"),
            new TypedTemplateDefinition(xamlPlatformInfo.PresentationNamespace + "HierarchicalDataTemplate", "DataType"),
            new TypedTemplateDefinition(xamlPlatformInfo.PresentationNamespace + "ItemContainerTemplate", "DataType"),
        };

        this.ControlTemplateDefinitions = new List<TypedTemplateDefinition>
        {
            new TypedTemplateDefinition(xamlPlatformInfo.PresentationNamespace + "ControlTemplate", "TargetType"),
        };

        this.ItemsPanelTemplateDefinitions = new List<UntypedTemplateDefinition>
        {
            new UntypedTemplateDefinition(xamlPlatformInfo.PresentationNamespace + "ItemsPanelTemplate"),
        };

        this.UnsupportedElements = new List<XName> { this.DataTriggerName, this.SetterName };

        this.ReadOnlyDependencyPropertyNotificationEvents = new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>>(() =>
        {
            var defaultReadOnlyDependencyPropertyNotificationEvents = xamlPlatformInfo.XamlPlatform switch
            {
                XamlPlatform.WPF => DefaultReadOnlyDependencyPropertyToNotificationEventWpf,
                XamlPlatform.UWP => DefaultReadOnlyDependencyPropertyToNotificationEventUwp,
                _ => throw this.CreateXamlPlatformNotSupportedException(),
            };

            return new CombinedDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>(bindingCompilerSettings.ReadOnlyDependencyPropertyToNotificationEvents ?? new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>(), defaultReadOnlyDependencyPropertyNotificationEvents.Value);
        });

        this.XamlTypeToSourceCodeNamespaces = new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>>(
            () =>
            {
                var defaultDefaultXamlTypeToSourceCodeNamespace = this.XamlPlatformInfo.XamlPlatform switch
                {
                    XamlPlatform.WPF => DefaultXamlTypeToSourceCodeNamespaceWpf,
                    XamlPlatform.UWP => DefaultXamlTypeToSourceCodeNamespaceUwp,
                    _ => throw this.CreateXamlPlatformNotSupportedException(),
                };

                return new CombinedDictionary<string, IReadOnlyDictionary<string, Namespace>>(bindingCompilerSettings.XamlTypeToSourceCodeTypes ?? new Dictionary<string, IReadOnlyDictionary<string, Namespace>>(), defaultDefaultXamlTypeToSourceCodeNamespace.Value);
            });

        this.OneWayBindingProperties = new Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(
            () =>
            {
                var defaultDefaultOneWayBindingProperties = this.XamlPlatformInfo.XamlPlatform switch
                {
                    XamlPlatform.WPF => DefaultOneWayBindingPropertiesWpf,
                    XamlPlatform.UWP => new Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(() => new Dictionary<string, IReadOnlyCollection<string>>()),
                    _ => throw this.CreateXamlPlatformNotSupportedException(),
                };

                return new CombinedDictionary<string, IReadOnlyCollection<string>>(bindingCompilerSettings.OneWayBindingProperties ?? new Dictionary<string, IReadOnlyCollection<string>>(), defaultDefaultOneWayBindingProperties.Value);
            });
        switch (xamlPlatformInfo.XamlPlatform)
        {
            case XamlPlatform.WPF:
                this.SundewBindingsXamlNamespace = "clr-namespace:Sundew.Xaml.Optimizations.Bindings;assembly=Sundew.Xaml.Wpf";
                this.UpdateSourceTriggerType = UpdateSourceTriggerTypeWpf;
                this.BindingModeType = BindingModeTypeWpf;
                break;
            case XamlPlatform.UWP:
                this.SundewBindingsXamlNamespace = "clr-namespace:Sundew.Xaml.Optimizations.Bindings;assembly=Sundew.Xaml.Uwp";
                this.UpdateSourceTriggerType = UpdateSourceTriggerTypeUwp;
                this.BindingModeType = BindingModeTypeUwp;
                break;
            default:
                throw this.CreateXamlPlatformNotSupportedException();
        }
    }

    public XamlPlatformInfo XamlPlatformInfo { get; }

    public IReadOnlyList<UntypedTemplateDefinition> ItemsPanelTemplateDefinitions { get; }

    public IReadOnlyList<TypedTemplateDefinition> ControlTemplateDefinitions { get; }

    public IReadOnlyList<TypedTemplateDefinition> DataTemplateDefinitions { get; }

    public IReadOnlyList<XName> UnsupportedElements { get; }

    public XName XKeyName { get; }

    public XName XClassName { get; }

    public XName ApplicationName { get; }

    public XName DataTriggerName { get; }

    public XName SetterName { get; }

    public XName DesignerDataContextName { get; }

    public XName SundewBindingsDataTypeName { get; }

    public XName SundewBindingsOptimizeBindingsName { get; }

    public string SundewBindingsXamlNamespace { get; }

    public XNamespace SundewXamlNamespace { get; }

    public string DefaultUsingStatements => StandardNamespacesDictionary[this.XamlPlatformInfo.XamlPlatform];

    public Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>> ReadOnlyDependencyPropertyNotificationEvents { get; }

    public Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>> XamlTypeToSourceCodeNamespaces { get; }

    public Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<string>>> OneWayBindingProperties { get; }

    public string UpdateSourceTriggerType { get; }

    public string BindingModeType { get; }

    private ArgumentOutOfRangeException CreateXamlPlatformNotSupportedException()
    {
        return new ArgumentOutOfRangeException(
            nameof(this.XamlPlatformInfo.XamlPlatform),
            this.XamlPlatformInfo.XamlPlatform,
            $"{this.XamlPlatformInfo.XamlPlatform} is not supported.");
    }

    private class CombinedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadOnlyDictionary<TKey, TValue> additionalDictionary;
        private readonly IReadOnlyDictionary<TKey, TValue> defaultDictionary;

        public CombinedDictionary(IReadOnlyDictionary<TKey, TValue> additionalDictionary, IReadOnlyDictionary<TKey, TValue> defaultDictionary)
        {
            this.additionalDictionary = additionalDictionary;
            this.defaultDictionary = defaultDictionary;
        }

        public int Count => this.additionalDictionary.Count + this.defaultDictionary.Count;

        public IEnumerable<TKey> Keys => this.additionalDictionary.Keys.Concat(this.defaultDictionary.Keys);

        public IEnumerable<TValue> Values => this.additionalDictionary.Values.Concat(this.defaultDictionary.Values);

        public TValue this[TKey key]
        {
            get
            {
                this.TryGetValue(key, out var value);
                return value;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.defaultDictionary.Concat(this.additionalDictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            if (this.additionalDictionary.ContainsKey(key))
            {
                return true;
            }

            return this.defaultDictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.additionalDictionary.TryGetValue(key, out value))
            {
                return true;
            }

            return this.defaultDictionary.TryGetValue(key, out value);
        }
    }
}