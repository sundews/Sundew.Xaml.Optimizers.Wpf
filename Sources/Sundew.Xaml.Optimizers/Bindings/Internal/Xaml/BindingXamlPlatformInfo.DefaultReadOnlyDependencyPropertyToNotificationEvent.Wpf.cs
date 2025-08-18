// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingXamlPlatformInfo.DefaultReadOnlyDependencyPropertyToNotificationEvent.Wpf.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings.Internal.Xaml;

using System;
using System.Collections.Generic;

internal partial class BindingXamlPlatformInfo
{
    /*private static readonly Lazy<IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>
        DefaultReadOnlyDependencyPropertyToNotificationEventWpf =
            new Lazy<IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>(() =>
                new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>()
                {
                    {
                        "System.Windows.Controls.ListBox.SelectedItems", new ReadOnlyDependencyPropertyToNotificationEvent("SelectionChanged", "System.Windows.Controls.SelectionChangedEventHandler")
                    },
                });*/
#nullable disable
    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>>
        DefaultReadOnlyDependencyPropertyToNotificationEventWpf =
            new Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>>(() =>
                new Dictionary<string, IReadOnlyDictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>>
                {
                    {
                        "PresentationFramework|System.Windows.FrameworkElement",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "ActualWidth", null },
                            { "ActualHeight", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.VisualStateManager",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "VisualStateGroups", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Window",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Data.CollectionViewSource",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "View", null },
                            { "CanChangeLiveSorting", null },
                            { "IsLiveSorting", null },
                            { "CanChangeLiveFiltering", null },
                            { "IsLiveFiltering", null },
                            { "CanChangeLiveGrouping", null },
                            { "IsLiveGrouping", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Navigation.NavigationWindow",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "BackStack", null },
                            { "ForwardStack", null },
                            { "CanGoBack", null },
                            { "CanGoForward", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.StickyNoteControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Author", null },
                            { "IsMouseOverAnchor", null },
                            { "StickyNoteType", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Button",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsDefaulted", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ComboBox",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "SelectionBoxItem", null },
                            { "SelectionBoxItemTemplate", null },
                            { "SelectionBoxItemStringFormat", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ComboBoxItem",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsHighlighted", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ContentControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasContent", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.DataGrid",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "RowHeaderActualWidth", null },
                            { "NewItemMargin", null },
                            { "NonFrozenColumnsViewportHorizontalOffset", null },
                            { "CellsPanelHorizontalOffset", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.DataGridCell",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Column", null },
                            { "IsReadOnly", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.DataGridColumn",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "ActualWidth", null },
                            { "IsAutoGenerated", null },
                            { "IsFrozen", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.DataGridRow",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "AlternationIndex", null },
                            { "IsEditing", null },
                            { "IsNewItem", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.DocumentViewer",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "ExtentWidth", null },
                            { "ExtentHeight", null },
                            { "ViewportWidth", null },
                            { "ViewportHeight", null },
                            { "CanMoveUp", null },
                            { "CanMoveDown", null },
                            { "CanMoveLeft", null },
                            { "CanMoveRight", null },
                            { "CanIncreaseZoom", null },
                            { "CanDecreaseZoom", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.FlowDocumentReader",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "PageCount", null },
                            { "PageNumber", null },
                            { "CanGoToPreviousPage", null },
                            { "CanGoToNextPage", null },
                            { "CanIncreaseZoom", null },
                            { "CanDecreaseZoom", null },
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.FlowDocumentScrollViewer",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "CanIncreaseZoom", null },
                            { "CanDecreaseZoom", null },
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Frame",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "CanGoBack", null },
                            { "CanGoForward", null },
                            { "BackStack", null },
                            { "ForwardStack", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.GridViewColumnHeader",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Column", null },
                            { "Role", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.HeaderedContentControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasHeader", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.HeaderedItemsControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasHeader", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.InkCanvas",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "ActiveEditingMode", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ItemsControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasItems", null },
                            { "IsGrouping", null },
                            { "AlternationIndex", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ListBox",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "SelectedItems", new ReadOnlyDependencyPropertyToNotificationEvent("SelectionChanged", "System.Windows.Controls.SelectionChangedEventHandler") },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.MenuItem",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Role", null },
                            { "IsPressed", null },
                            { "IsHighlighted", null },
                            { "IsSuspendingPopupAnimation", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.PasswordBox",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ScrollViewer",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "ComputedHorizontalScrollBarVisibility", null },
                            { "ComputedVerticalScrollBarVisibility", null },
                            { "VerticalOffset", null },
                            { "HorizontalOffset", null },
                            { "ContentVerticalOffset", null },
                            { "ContentHorizontalOffset", null },
                            { "ExtentWidth", null },
                            { "ExtentHeight", null },
                            { "ScrollableWidth", null },
                            { "ScrollableHeight", null },
                            { "ViewportWidth", null },
                            { "ViewportHeight", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.FlowDocumentPageViewer",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "CanIncreaseZoom", null },
                            { "CanDecreaseZoom", null },
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.TabControl",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "SelectedContent", null },
                            { "SelectedContentTemplate", null },
                            { "SelectedContentTemplateSelector", null },
                            { "SelectedContentStringFormat", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.TabItem",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "TabStripPlacement", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.ToolBar",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Orientation", null },
                            { "HasOverflowItems", null },
                            { "IsOverflowItem", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.TreeView",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "SelectedItem", null },
                            { "SelectedValue", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.TreeViewItem",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Viewport3D",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "Children", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.ButtonBase",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsPressed", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.CalendarButton",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasSelectedDays", null },
                            { "IsInactive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.CalendarDayButton",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsToday", null },
                            { "IsSelected", null },
                            { "IsInactive", null },
                            { "IsBlackedOut", null },
                            { "IsHighlighted", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.DataGridColumnHeader",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "DisplayIndex", null },
                            { "CanUserSort", null },
                            { "SortDirection", null },
                            { "IsFrozen", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.DataGridRowHeader",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsRowSelected", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.DocumentViewerBase",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "PageCount", null },
                            { "MasterPageNumber", null },
                            { "CanGoToPreviousPage", null },
                            { "CanGoToNextPage", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.Popup",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "HasDropShadow", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.Selector",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.TextBoxBase",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsSelectionActive", null },
                        }
                    },
                    {
                        "PresentationFramework|System.Windows.Controls.Primitives.Thumb",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsDragging", null },
                        }
                    },
                    {
                        "PresentationCore|System.Windows.ContentElement",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsMouseDirectlyOver", null },
                            { "IsMouseOver", null },
                            { "IsStylusOver", null },
                            { "IsKeyboardFocusWithin", null },
                            { "IsMouseCaptured", null },
                            { "IsMouseCaptureWithin", null },
                            { "IsStylusDirectlyOver", null },
                            { "IsStylusCaptured", null },
                            { "IsStylusCaptureWithin", null },
                            { "IsKeyboardFocused", null },
                            { "AreAnyTouchesDirectlyOver", null },
                            { "AreAnyTouchesOver", null },
                            { "AreAnyTouchesCaptured", null },
                            { "AreAnyTouchesCapturedWithin", null },
                            { "IsFocused", null },
                        }
                    },
                    {
                        "PresentationCore|System.Windows.UIElement",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsMouseDirectlyOver", null },
                            { "IsMouseOver", null },
                            { "IsStylusOver", null },
                            { "IsKeyboardFocusWithin", null },
                            { "IsMouseCaptured", null },
                            { "IsMouseCaptureWithin", null },
                            { "IsStylusDirectlyOver", null },
                            { "IsStylusCaptured", null },
                            { "IsStylusCaptureWithin", null },
                            { "IsKeyboardFocused", null },
                            { "AreAnyTouchesDirectlyOver", null },
                            { "AreAnyTouchesOver", null },
                            { "AreAnyTouchesCaptured", null },
                            { "AreAnyTouchesCapturedWithin", null },
                            { "IsFocused", null },
                            { "IsVisible", null },
                        }
                    },
                    {
                        "PresentationCore|System.Windows.UIElement3D",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsMouseDirectlyOver", null },
                            { "IsMouseOver", null },
                            { "IsStylusOver", null },
                            { "IsKeyboardFocusWithin", null },
                            { "IsMouseCaptured", null },
                            { "IsMouseCaptureWithin", null },
                            { "IsStylusDirectlyOver", null },
                            { "IsStylusCaptured", null },
                            { "IsStylusCaptureWithin", null },
                            { "IsKeyboardFocused", null },
                            { "AreAnyTouchesDirectlyOver", null },
                            { "AreAnyTouchesOver", null },
                            { "AreAnyTouchesCaptured", null },
                            { "AreAnyTouchesCapturedWithin", null },
                            { "IsFocused", null },
                            { "IsVisible", null },
                        }
                    },
                    {
                        "PresentationCore|System.Windows.Interop.D3DImage",
                        new Dictionary<string, ReadOnlyDependencyPropertyToNotificationEvent>
                        {
                            { "IsFrontBufferAvailable", null },
                        }
                    },
                });
}