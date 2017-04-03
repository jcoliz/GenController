using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IotHello.Uwp.Screens
{
    public sealed partial class Settings : Page
    {
        public Portable.ViewModels.SettingsViewModel VM { get; } = new Portable.ViewModels.SettingsViewModel();

        Portable.ViewModels.MainViewModel MainVM = new Portable.ViewModels.MainViewModel();

        public Settings()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Current.Measurement.LogEvent("Screen.Settings");
            App.Current.Tick += App_Tick;
            base.OnNavigatedTo(e);
        }

        private void App_Tick(object sender, object e)
        {
            Bindings.Update();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.Current.Tick -= App_Tick;
            base.OnNavigatedFrom(e);
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            VM.Commit();
            App.Current.Measurement.LogEvent("Time.Set", $"Time={VM.DT}");
            Frame.GoBack();
        }
    }
}
