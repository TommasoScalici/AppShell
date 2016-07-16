using Windows.UI.Xaml.Controls;

namespace AppShell.TestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            TommasoScalici.AppShell.AppShell.Current.AppFrame.Navigate(typeof(HomePage));
        }
    }
}
