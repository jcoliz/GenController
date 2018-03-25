using Commonality;
using GenController.Uwp.Platform;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GenController.Uwp
{
    public sealed partial class MainPage : Page
    {
        public Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                ex.Source = "MX0";
                Logger?.LogError(ex);
                VM_ExceptionRaised(this, ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.ExceptionRaised += VM_ExceptionRaised;
            App.Current.Tick += App_Tick;
            VM.Update();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VM.ExceptionRaised -= VM_ExceptionRaised;
            App.Current.Tick -= App_Tick;
            base.OnNavigatedFrom(e);
        }

        private void VM_ExceptionRaised(object sender, Exception ex)
        {
            string message = ex.Message;
            if (!string.IsNullOrEmpty(ex.Source))
                message += $" (Code {ex.Source})";
            var ignore = new MessageDialog(message, "SORRY").ShowAsync();
        }

        private void App_Tick(object sender, object e)
        {
            VM.Update();
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

        private void PeriodsView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Screens.EditSchedule),e.ClickedItem);
        }

        private ILogger Logger => Service.TryGet<ILogger>();
    }
}
