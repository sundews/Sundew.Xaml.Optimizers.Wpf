// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlPlatformInfo.DefaultXamlTypeToSourceCodeNamespace.Wpf.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System;
using System.Collections.Generic;

internal partial class BindingXamlPlatformInfo
{
    private const string UpdateSourceTriggerTypeWpf = "System.Windows.Data.UpdateSourceTrigger";
    private const string BindingModeTypeWpf = "System.Windows.Data.BindingMode";
    private static readonly Namespace SystemWindows = new Namespace("System.Windows", "PresentationFramework");

    private static readonly Namespace SystemWindowsControls =
        new Namespace("System.Windows.Controls", "PresentationFramework");

    private static readonly Namespace SystemWindowsControlsPrimitives =
        new Namespace("System.Windows.Controls.Primitives", "PresentationFramework");

    private static readonly Namespace SystemWindowsDocuments =
        new Namespace("System.Windows.Documents", "PresentationFramework");

    private static readonly Namespace SystemWindowsNavigation =
        new Namespace("System.Windows.Navigation", "PresentationFramework");

    private static readonly Namespace SystemWindowsInterop =
        new Namespace("System.Windows.Interop", "PresentationFramework");

    private static readonly Namespace SystemWindowsShapes =
        new Namespace("System.Windows.Shapes", "PresentationFramework");

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>>
        DefaultXamlTypeToSourceCodeNamespaceWpf =
            new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, Namespace>>>(() =>
                new Dictionary<string, IReadOnlyDictionary<string, Namespace>>()
                {
                    {
                        "http://schemas.microsoft.com/winfx/2006/xaml/presentation",
                        new Dictionary<string, Namespace>()
                        {
                            { "Application", SystemWindows },
                            { "ResourceDictionary", SystemWindows },
                            { "AccessText", SystemWindowsControls },
                            { "ActiveXHost", SystemWindowsInterop },
                            { "AdornedElementPlaceholder", SystemWindowsControls },
                            { "AdornerDecorator", SystemWindowsDocuments },
                            { "AdornerLayer", SystemWindowsDocuments },
                            { "Border", SystemWindowsControls },
                            { "BulletDecorator", SystemWindowsControlsPrimitives },
                            { "Button", SystemWindowsControls },
                            { "Calendar", SystemWindowsControls },
                            { "CalendarButton", SystemWindowsControlsPrimitives },
                            { "CalendarDayButton", SystemWindowsControlsPrimitives },
                            { "CalendarItem", SystemWindowsControlsPrimitives },
                            { "Canvas", SystemWindowsControls },
                            { "CheckBox", SystemWindowsControls },
                            { "ComboBox", SystemWindowsControls },
                            { "ComboBoxItem", SystemWindowsControls },
                            { "ContentControl", SystemWindowsControls },
                            { "ContentPresenter", SystemWindowsControls },
                            { "ContextMenu", SystemWindowsControls },
                            { "Control", SystemWindowsControls },
                            { "DataGrid", SystemWindowsControls },
                            { "DataGridCell", SystemWindowsControls },
                            { "DataGridCellsPanel", SystemWindowsControls },
                            { "DataGridCellsPresenter", SystemWindowsControlsPrimitives },
                            { "DataGridColumnHeader", SystemWindowsControlsPrimitives },
                            { "DataGridColumnHeadersPresenter", SystemWindowsControlsPrimitives },
                            { "DataGridDetailsPresenter", SystemWindowsControlsPrimitives },
                            { "DataGridRow", SystemWindowsControls },
                            { "DataGridRowHeader", SystemWindowsControlsPrimitives },
                            { "DataGridRowsPresenter", SystemWindowsControlsPrimitives },
                            { "DatePicker", SystemWindowsControls },
                            { "DatePickerTextBox", SystemWindowsControlsPrimitives },
                            { "Decorator", SystemWindowsControls },
                            { "DockPanel", SystemWindowsControls },
                            { "DocumentPageView", SystemWindowsControlsPrimitives },
                            { "DocumentReference", SystemWindowsDocuments },
                            { "DocumentViewer", SystemWindowsControls },
                            { "DocumentViewerBase", SystemWindowsControlsPrimitives },
                            { "Ellipse", SystemWindowsShapes },
                            { "Expander", SystemWindowsControls },
                            { "FixedPage", SystemWindowsDocuments },
                            { "FlowDocumentPageViewer", SystemWindowsControls },
                            { "FlowDocumentReader", SystemWindowsControls },
                            { "FlowDocumentScrollViewer", SystemWindowsControls },
                            { "Frame", SystemWindowsControls },
                            { "Glyphs", SystemWindowsDocuments },
                            { "Grid", SystemWindowsControls },
                            { "GridSplitter", SystemWindowsControls },
                            { "GridViewColumnHeader", SystemWindowsControls },
                            { "GridViewHeaderRowPresenter", SystemWindowsControls },
                            { "GridViewRowPresenter", SystemWindowsControls },
                            { "GridViewRowPresenterBase", SystemWindowsControlsPrimitives },
                            { "GroupBox", SystemWindowsControls },
                            { "GroupItem", SystemWindowsControls },
                            { "HeaderedContentControl", SystemWindowsControls },
                            { "HeaderedItemsControl", SystemWindowsControls },
                            { "HighlightVisual", SystemWindowsDocuments },
                            { "Image", SystemWindowsControls },
                            { "InkCanvas", SystemWindowsControls },
                            { "InkPresenter", SystemWindowsControls },
                            { "ItemsControl", SystemWindowsControls },
                            { "ItemsPresenter", SystemWindowsControls },
                            { "Label", SystemWindowsControls },
                            { "Line", SystemWindowsShapes },
                            { "ListBox", SystemWindowsControls },
                            { "ListBoxItem", SystemWindowsControls },
                            { "ListView", SystemWindowsControls },
                            { "ListViewItem", SystemWindowsControls },
                            { "MediaElement", SystemWindowsControls },
                            { "Menu", SystemWindowsControls },
                            { "MenuItem", SystemWindowsControls },
                            { "NavigationWindow", SystemWindowsNavigation },
                            { "Page", SystemWindowsControls },
                            { "PageContent", SystemWindowsDocuments },
                            { "PageFunction<T>", SystemWindowsNavigation },
                            { "Panel", SystemWindowsControls },
                            { "PasswordBox", SystemWindowsControls },
                            { "Path", SystemWindowsShapes },
                            { "Polygon", SystemWindowsShapes },
                            { "Polyline", SystemWindowsShapes },
                            { "Popup", SystemWindowsControlsPrimitives },
                            { "ProgressBar", SystemWindowsControls },
                            { "RadioButton", SystemWindowsControls },
                            { "Rectangle", SystemWindowsShapes },
                            { "RepeatButton", SystemWindowsControlsPrimitives },
                            { "ResizeGrip", SystemWindowsControlsPrimitives },
                            { "RichTextBox", SystemWindowsControls },
                            { "ScrollBar", SystemWindowsControlsPrimitives },
                            { "ScrollContentPresenter", SystemWindowsControls },
                            { "ScrollViewer", SystemWindowsControls },
                            { "SelectiveScrollingGrid", SystemWindowsControlsPrimitives },
                            { "Selector", SystemWindowsControlsPrimitives },
                            { "Separator", SystemWindowsControls },
                            { "Slider", SystemWindowsControls },
                            { "StackPanel", SystemWindowsControls },
                            { "StatusBar", SystemWindowsControlsPrimitives },
                            { "StatusBarItem", SystemWindowsControlsPrimitives },
                            { "StickyNoteControl", SystemWindowsControls },
                            { "TabControl", SystemWindowsControls },
                            { "TabItem", SystemWindowsControls },
                            { "TabPanel", SystemWindowsControlsPrimitives },
                            { "TextBlock", SystemWindowsControls },
                            { "TextBox", SystemWindowsControls },
                            { "Thumb", SystemWindowsControlsPrimitives },
                            { "TickBar", SystemWindowsControlsPrimitives },
                            { "ToggleButton", SystemWindowsControlsPrimitives },
                            { "ToolBar", SystemWindowsControls },
                            { "ToolBarOverflowPanel", SystemWindowsControlsPrimitives },
                            { "ToolBarPanel", SystemWindowsControlsPrimitives },
                            { "ToolBarTray", SystemWindowsControls },
                            { "ToolTip", SystemWindowsControls },
                            { "Track", SystemWindowsControlsPrimitives },
                            { "TreeView", SystemWindowsControls },
                            { "TreeViewItem", SystemWindowsControls },
                            { "UniformGrid", SystemWindowsControlsPrimitives },
                            { "UserControl", SystemWindowsControls },
                            { "Viewbox", SystemWindowsControls },
                            { "Viewport3D", SystemWindowsControls },
                            { "VirtualizingPanel", SystemWindowsControls },
                            { "VirtualizingStackPanel", SystemWindowsControls },
                            { "WebBrowser", SystemWindowsControls },
                            { "Window", SystemWindows },
                            { "WrapPanel", SystemWindowsControls },
                            { "BlockUIContainer", SystemWindowsDocuments },
                            { "Bold", SystemWindowsDocuments },
                            { "ColumnDefinition", SystemWindowsControls },
                            { "Figure", SystemWindowsDocuments },
                            { "FixedDocument", SystemWindowsDocuments },
                            { "FixedDocumentSequence", SystemWindowsDocuments },
                            { "Floater", SystemWindowsDocuments },
                            { "FlowDocument", SystemWindowsDocuments },
                            { "Hyperlink", SystemWindowsDocuments },
                            { "InlineUIContainer", SystemWindowsDocuments },
                            { "Italic", SystemWindowsDocuments },
                            { "LineBreak", SystemWindowsDocuments },
                            { "List", SystemWindowsDocuments },
                            { "ListItem", SystemWindowsDocuments },
                            { "Paragraph", SystemWindowsDocuments },
                            { "RowDefinition", SystemWindowsControls },
                            { "Run", SystemWindowsDocuments },
                            { "Section", SystemWindowsDocuments },
                            { "Span", SystemWindowsDocuments },
                            { "Table", SystemWindowsDocuments },
                            { "TableCell", SystemWindowsDocuments },
                            { "TableColumn", SystemWindowsDocuments },
                            { "TableRow", SystemWindowsDocuments },
                            { "TableRowGroup", SystemWindowsDocuments },
                            { "Underline", SystemWindowsDocuments },
                        }
                    },
                });
}