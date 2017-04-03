using IotHello.Uwp.Platform;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IotHello.Uwp.Screens
{
    public sealed partial class Settings : Page
    {
        public Portable.ViewModels.SettingsViewModel VM { get; } = new Portable.ViewModels.SettingsViewModel();

        public Settings()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                string code = "EX0";
                App.Current.Measurement.Error(code, ex);
                VM_ExceptionRaised(this, new ManiaLabs.Portable.Base.ExceptionArgs(ex, code));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.ExceptionRaised += VM_ExceptionRaised;
            App.Current.Measurement.LogEvent("Screen.Settings");
            App.Current.Tick += App_Tick;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VM.ExceptionRaised -= VM_ExceptionRaised;
            App.Current.Tick -= App_Tick;
            base.OnNavigatedFrom(e);
        }

        private void VM_ExceptionRaised(object sender, ManiaLabs.Portable.Base.ExceptionArgs e)
        {
            string message = e.ex.Message;
            if (!string.IsNullOrEmpty(e.code))
                message += $" (Code {e.code})";
            var ignore = new MessageDialog(message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
        }

        private void App_Tick(object sender, object e)
        {
            Bindings.Update();
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
