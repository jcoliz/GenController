using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IotHello.Uwp
{
    public sealed partial class MainPage : Page
    {
        public Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Current.Tick += App_Tick;
            VM.Update();
            base.OnNavigatedTo(e);
        }

        private void App_Tick(object sender, object e)
        {
            VM.Update();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.Current.Tick -= App_Tick;
            base.OnNavigatedFrom(e);
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Screens.Settings));
        }

        private void Button_Logs_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Screens.Logs));
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Screens.EditSchedule));
        }

        private void Button_Power_Click(object sender, RoutedEventArgs e)
        {
            VM.Controller.ToggleDisable();
        }

        private void PeriodsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Screens.EditSchedule),e.ClickedItem);
        }
    }
}
