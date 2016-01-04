using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            test.SetBinding(VisibilityProperty, new Binding
            {
                Converter = (IValueConverter)Resources["BooleanToVisibilityConverter"],
                Path = new PropertyPath("IsPaneOpen"),
                Source = appShell.ShellSplitView
            });
        }
    }
}
