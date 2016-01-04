using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TommasoScalici.AppShell
{
    public sealed partial class PageFooter : UserControl
    {
        public PageFooter()
        {
            InitializeComponent();
        }


        public UIElement FooterContent
        {
            get { return (UIElement)GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.Register("FooterContent",
            typeof(UIElement), typeof(PageFooter), new PropertyMetadata(DependencyProperty.UnsetValue));

    }
}
