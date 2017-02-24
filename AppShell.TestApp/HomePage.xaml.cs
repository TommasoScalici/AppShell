using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppShell.TestApp
{
    public sealed partial class HomePage : Page
    {
        public HomePage() => InitializeComponent();


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            switch (e.Parameter as string)
            {
                case "Info":
                    contentTextBlock.Text = "Info";
                    break;
                case "Profile":
                    contentTextBlock.Text = "Profile";
                    break;
                case "Settings":
                    contentTextBlock.Text = "Settings";
                    break;
            }
        }
    }
}
