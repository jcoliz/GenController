using Commonality;
using GenController.Portable.ViewModels;
using GenController.Uwp.Platform;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GenController.Uwp.Screens
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
                ex.Source = "LX0";
                Logger?.LogError(ex);
                VM_ExceptionRaised(this, ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Logger?.LogEvent("Screen.Logs");
            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => 
            {
                await VM.Load();
                await Task.Delay(200);
                await VM.SelectSession(VM.Sessions.First());
            });
            VM.ExceptionRaised += VM_ExceptionRaised;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VM.ExceptionRaised -= VM_ExceptionRaised;
            base.OnNavigatedFrom(e);
        }

        private void VM_ExceptionRaised(object sender, Exception ex)
        {
            string message = ex.Message;
            if (!string.IsNullOrEmpty(ex.Source))
                message += $" (Code {ex.Source})";
            var ignore = new MessageDialog(message, "SORRY").ShowAsync();
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (e.AddedItems?.FirstOrDefault() != null)
            {
                lv.ScrollIntoView(e.AddedItems.First());
            }
        }

        private ILogger Logger => Service.TryGet<ILogger>();
    }
}
