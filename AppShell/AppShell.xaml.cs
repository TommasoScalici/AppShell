using System.Collections.Generic;
using System.Linq;

using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TommasoScalici.AppShell
{
    public sealed partial class AppShell : UserControl
    {
        List<MenuItem> menuItems = new List<MenuItem>();


        public AppShell()
        {
            InitializeComponent();
            Current = this;

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


        public event TypedEventHandler<AppShell, Rect> TogglePaneButtonRectChanged;


        public static AppShell Current { get; private set; }

        public IEnumerable<MenuItem> BottomMenuItems { get; private set; }
        public IEnumerable<MenuItem> TopMenuItems { get; private set; }
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


        IEnumerable<MenuListView> GetMenuListViews()
        {
            if (shellSplitView.Pane is MenuListView)
                yield return shellSplitView.Pane as MenuListView;
            else if (shellSplitView.Pane is Panel)
            {
                foreach (var child in (shellSplitView.Pane as Panel).Children)
                {
                    if (child is MenuListView)
                        yield return child as MenuListView;
                }
            }
        }

        void OnPaneSizeChanged()
        {
            if (shellSplitView.DisplayMode == SplitViewDisplayMode.Inline || shellSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
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
            var menuItems = GetMenuListViews().Select(menuListView => menuListView.Items).Cast<MenuItem>();
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
            var menuListViews = GetMenuListViews();

            foreach (var menuListView in menuListViews)
            {
                foreach (var menuItem in menuListView.Items)
                {
                    var container = menuListView.ContainerFromItem(item);
                    menuListView.SetSelectedItem(container as ListViewItem);
                }
            }
        }
    }
}
