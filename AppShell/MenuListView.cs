using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace TommasoScalici.AppShell
{
    public sealed class MenuListView : ListView
    {
        List<Assembly> userAssemblies = new List<Assembly>();
        SplitView splitViewHost;


        public MenuListView()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                BorderBrush = (Brush)Resources["SystemControlBackgroundAccentBrush"];
                ItemContainerStyle = (Style)AppShell.Current.Resources["MenuItemContainerStyle"];
                ItemTemplate = (DataTemplate)AppShell.Current.Resources["MenuItemTemplate"];
            }

            IsItemClickEnabled = true;
            SelectionMode = ListViewSelectionMode.Single;

            ContainerContentChanging += MenuItemContainerContentChanging;
            Loaded += MenuListviewLoaded;
            ItemClick += ItemClickedHandler;
            ItemInvoked += MenuItemInvoked;
            SelectionChanged += MenuListViewSelectionChanged;
        }


        public event EventHandler<ListViewItem> ItemInvoked;


        public void SetSelectedItem(ListViewItem item)
        {
            int index = -1;

            if (item != null)
                index = IndexFromContainer(item);

            for (int i = 0; i < Items.Count; i++)
            {
                var listViewItem = (ListViewItem)ContainerFromIndex(i);

                if (listViewItem != null)
                    listViewItem.IsSelected = i == index;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!DesignMode.DesignModeEnabled)
            {
                for (int i = 0; i < ItemContainerTransitions.Count; i++)
                {
                    if (ItemContainerTransitions[i] is EntranceThemeTransition)
                        ItemContainerTransitions.RemoveAt(i);
                }
            }
        }

        void InvokeItem(object focusedItem)
        {
            var navMenuItem = ((focusedItem as ListViewItem)?.Content as MenuItem);

            if (navMenuItem != null && navMenuItem.IsSelectable)
                SetSelectedItem(focusedItem as ListViewItem);

            ItemInvoked(this, focusedItem as ListViewItem);

            if (splitViewHost.IsPaneOpen && (
                splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                splitViewHost.DisplayMode == SplitViewDisplayMode.Overlay))
            {
                splitViewHost.IsPaneOpen = false;

                if (focusedItem is ListViewItem)
                    (focusedItem as ListViewItem)?.Focus(FocusState.Programmatic);
            }
        }

        void ItemClickedHandler(object sender, ItemClickEventArgs e)
        {
            var item = ContainerFromItem(e.ClickedItem);
            InvokeItem(item);
        }

        void MenuItemContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (!args.InRecycleQueue && args.Item != null && args.Item is MenuItem)
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, ((MenuItem)args.Item).Label);
            else
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty);
        }

        void MenuItemInvoked(object sender, ListViewItem e)
        {
            var item = (sender as MenuListView)?.ItemFromContainer(e) as MenuItem;
            Type destinationPageType = null;

            item?.RaiseClick();

            if (!string.IsNullOrEmpty(item.DestinationPage))
            {
                foreach (var assembly in userAssemblies)
                {
                    destinationPageType = assembly.GetType(item.DestinationPage);

                    if (destinationPageType != null)
                        break;
                }

                if (destinationPageType != null)
                    AppShell.Current.AppFrame.Navigate(destinationPageType, item.Arguments);
            }
        }

        void MenuListviewLoaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                var folder = Package.Current.InstalledLocation;

                foreach (var file in await folder.GetFilesAsync())
                {
                    if ((file.FileType != ".dll" && file.FileType != ".exe") ||
                         file.DisplayName.StartsWith("Clr", StringComparison.Ordinal) ||
                         file.DisplayName.StartsWith("Microsoft", StringComparison.Ordinal) ||
                         file.DisplayName.StartsWith("System", StringComparison.Ordinal))
                        continue;

                    userAssemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }

                var parent = VisualTreeHelper.GetParent(this);

                while (parent != null && !(parent is SplitView))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null)
                {
                    splitViewHost = parent as SplitView;
                    splitViewHost.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty, (s, a) => OnPaneToggled());

                    OnPaneToggled();
                }
            });
        }

        void MenuListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var menuListView = sender as MenuListView;
            var selectedMenuItem = menuListView?.SelectedItem as MenuItem;

            if (selectedMenuItem != null && !selectedMenuItem.IsSelectable)
                menuListView.SelectedItem = e.RemovedItems.FirstOrDefault();
        }

        void OnPaneToggled()
        {
            if (splitViewHost.IsPaneOpen)
            {
                ItemsPanelRoot?.ClearValue(WidthProperty);
                ItemsPanelRoot?.ClearValue(HorizontalAlignmentProperty);
            }
            else if (splitViewHost.DisplayMode == SplitViewDisplayMode.CompactInline ||
                     splitViewHost.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                ItemsPanelRoot?.SetValue(WidthProperty, splitViewHost.CompactPaneLength);
                ItemsPanelRoot?.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
            }
        }
    }
}
