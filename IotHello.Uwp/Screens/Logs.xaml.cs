using IotHello.Portable.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    /// Log viewer screen
    /// </summary>
    public sealed partial class Logs : Page
    {
        public LogsViewModel VM { get; } = new LogsViewModel();

        public Logs()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                string code = "LX0";
                string message = ex.Message + $"(Code {code})";
                App.Current.Measurement.Error(code, ex);
                var ignore = new MessageDialog(message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.Current.Measurement.LogEvent("Screen.Logs");
            var ignore = VM.Load();
            VM.ExceptionRaised += VM_ExceptionRaised;
        }

        private void VM_ExceptionRaised(object sender, ManiaLabs.Portable.Base.ExceptionArgs e)
        {
            string message = e.ex.Message;
            if (!string.IsNullOrEmpty(e.code))
                message += $" (Code {e.code})";
            var ignore = new MessageDialog(message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dt = e?.ClickedItem as DateTime?;
            if (dt.HasValue == true)
                await VM.SelectSession(dt.Value);
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
