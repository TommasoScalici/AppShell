using System.Collections.Generic;
using System.Linq;

using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TommasoScalici.AppShell
{
    public sealed partial class AppShell : UserControl
    {
        SplitViewDisplayMode previousSplitViewDisplayMode;
        List<MenuItem> menuItems = new List<MenuItem>();
        List<VisualStateGroup> visualStateGroups = new List<VisualStateGroup>();


        public AppShell()
        {
            InitializeComponent();

            if (!DesignMode.DesignModeEnabled)
            {
                Current = this;

                LayoutUpdated += (sender, args) => OnPaneSizeChanged();

                Loaded += (sender, args) =>
                {
                    togglePaneButton.Focus(FocusState.Programmatic);
                    OnPaneSizeChanged();
                };

                SystemNavigationManager.GetForCurrentView().BackRequested += (sender, e) =>
                {
                    if (!e.Handled && AppFrame.CanGoBack)
                    {
                        e.Handled = true;
                        AppFrame.GoBack();
                    }
                };
            }
        }


        public event TypedEventHandler<AppShell, bool> PaneStateChanged;
        public event TypedEventHandler<AppShell, Rect> TogglePaneButtonRectChanged;


        public static AppShell Current { get; private set; }

        public Frame AppFrame { get { return frame; } }
        public Rect PaneToggleButtonRect { get; private set; }
        public SplitView ShellSplitView { get { return shellSplitView; } }

        public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached("Menu",
            typeof(UIElement), typeof(AppShell), new PropertyMetadata(DependencyProperty.UnsetValue));


        public static UIElement GetMenu(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(MenuProperty);
        }

        public static void SetMenu(DependencyObject obj, UIElement value)
        {
            obj.SetValue(MenuProperty, value);
            Current.shellSplitView.Pane = value;
        }

        public void HideMenu()
        {
            var groups = VisualStateManager.GetVisualStateGroups(rootGrid);

            foreach (var group in groups)
                visualStateGroups.Add(group);

            groups.Clear();

            previousSplitViewDisplayMode = ShellSplitView.DisplayMode;
            ShellSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
            ShellSplitView.IsPaneOpen = false;
            togglePaneButton.Visibility = Visibility.Collapsed;

            if (ShellSplitView.Pane != null)
                ShellSplitView.Pane.Visibility = Visibility.Collapsed;

            OnPaneSizeChanged();
        }

        public void ShowMenu()
        {
            var groups = VisualStateManager.GetVisualStateGroups(rootGrid);

            foreach (var group in visualStateGroups)
                groups.Add(group);

            visualStateGroups.Clear();

            togglePaneButton.Visibility = Visibility.Visible;

            if (ShellSplitView.Pane != null)
            {
                ShellSplitView.DisplayMode = previousSplitViewDisplayMode;
                ShellSplitView.Pane.Visibility = Visibility.Visible;
            }

            OnPaneSizeChanged();
        }

        IEnumerable<MenuListView> GetMenuListViews(UIElement element)
        {
            if (element is MenuListView)
                yield return element as MenuListView;

            if (element is ContentControl)
            {
                var content = (element as ContentControl)?.Content as UIElement;

                if (content != null)
                    foreach (var menuListView in GetMenuListViews(content))
                        yield return menuListView;
            }
            if (element is Panel)
            {
                var children = (element as Panel)?.Children;

                if (children != null)
                    foreach (var child in children)
                        foreach (var menuListView in GetMenuListViews(child))
                            yield return menuListView;
            }
        }

        void OnPaneSizeChanged()
        {
            if (shellSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = togglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, togglePaneButton.ActualWidth, togglePaneButton.ActualHeight));
                PaneToggleButtonRect = rect;
            }
            else
                PaneToggleButtonRect = new Rect();

            TogglePaneButtonRectChanged?.Invoke(Current, PaneToggleButtonRect);
        }

        void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            var menuItems = from menuListView in GetMenuListViews(shellSplitView.Pane)
                            from MenuItem menuItem in menuListView.Items
                            select menuItem;

            var item = menuItems.SingleOrDefault(p => p.DestinationPage == e.SourcePageType.FullName);
            var snm = SystemNavigationManager.GetForCurrentView();

            if (item == null && AppFrame.BackStackDepth > 0)
            {
                foreach (var entry in AppFrame.BackStack.Reverse())
                {
                    item = menuItems.SingleOrDefault(p => p.DestinationPage == entry.SourcePageType.FullName);

                    if (item != null)
                        break;
                }
            }

            if (item != null && item.IsSelectable)
                SetSelectedMenuItem(item);

            snm.AppViewBackButtonVisibility = AppFrame.CanGoBack ?
                                              AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        void SetSelectedMenuItem(MenuItem item)
        {
            var menuListViews = GetMenuListViews(shellSplitView.Pane);

            foreach (var menuListView in menuListViews)
            {
                foreach (var menuItem in menuListView.Items)
                {
                    var container = menuListView.ContainerFromItem(item);
                    menuListView.SetSelectedItem(container as ListViewItem);
                }
            }
        }

        void TogglePaneButtonChecked(object sender, RoutedEventArgs e)
        {
            OnPaneSizeChanged();
            PaneStateChanged?.Invoke(Current, shellSplitView.IsPaneOpen);
        }

        void TogglePaneButtonUnchecked(object sender, RoutedEventArgs e)
        {
            OnPaneSizeChanged();
            PaneStateChanged?.Invoke(Current, shellSplitView.IsPaneOpen);
        }
    }
}
