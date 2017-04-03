using ManiaLabs.Helpers;
using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IotHello.Uwp.Screens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
