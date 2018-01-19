using Common;
using IotHello.Uwp.Platform;
using System;
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
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                string code = "EX0";
                Logger?.Error(code, ex);
                VM_ExceptionRaised(this, new Common.ViewModelBase.ExceptionArgs(ex, code));
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

        private void VM_ExceptionRaised(object sender, Common.ViewModelBase.ExceptionArgs e)
        {
            string message = e.ex.Message;
            if (!string.IsNullOrEmpty(e.code))
                message += $" (Code {e.code})";
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
