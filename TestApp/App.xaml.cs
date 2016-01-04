using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace TestApp
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window.Current.Content = Window.Current.Content as MainPage ?? new MainPage();

            Window.Current.Activate();
        }

    }
}
