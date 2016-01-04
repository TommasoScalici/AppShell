using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TommasoScalici.AppShell
{
    public sealed partial class PageHeader : UserControl
    {
        public PageHeader()
        {
            InitializeComponent();

            Loaded += (s, a) =>
            {
                AppShell.Current.TogglePaneButtonRectChanged += CurrentTogglePaneButtonSizeChanged;
                titleBar.Margin = new Thickness(AppShell.Current.PaneToggleButtonRect.Right, 0, 0, 0);
            };
        }


        public UIElement HeaderContent
        {
            get { return (UIElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent",
            typeof(UIElement), typeof(PageHeader), new PropertyMetadata(DependencyProperty.UnsetValue));


        void CurrentTogglePaneButtonSizeChanged(AppShell sender, Rect args)
        {
            titleBar.Margin = new Thickness(args.Right, 0, 0, 0);
        }
    }
}
