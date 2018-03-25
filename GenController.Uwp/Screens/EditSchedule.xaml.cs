using Commonality;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GenController.Uwp.Screens
{
    public sealed partial class EditSchedule : Page
    {

        Portable.ViewModels.EditScheduleViewModel VM = new Portable.ViewModels.EditScheduleViewModel();

        private string Purpose => VM.WillAdd ? "Add Schedule Block" : "Edit Schedule";

        public EditSchedule()
        {
            try
            {
                this.InitializeComponent();
            }
            catch (Exception ex)
            {
                ex.Source = "EX0";
                Logger?.LogError(ex);
                VM_ExceptionRaised(this, ex);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.ExceptionRaised += VM_ExceptionRaised;
            if (e.Parameter != null && e.Parameter is Portable.Models.GenPeriod)
            {
                VM.Original = e.Parameter as Portable.Models.GenPeriod;
            }
            Logger?.LogEvent("Screen.EditSchedule", $"Add={VM.WillAdd}");
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

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VM.Commit();
                Logger?.LogEvent("Schedule.Commit", $"Start={VM.Period.SerializeKey}",$"Delete={VM.WillDelete}");
                Frame.GoBack();
            }
            catch (Exception ex)
            {
                Logger?.LogEvent("Schedule.CommitFailed",$"Reason={ex.Message}");
                var ignore = new MessageDialog(ex.Message, "SORRY").ShowAsync();
            }
        }

        private ILogger Logger => Service.TryGet<ILogger>();
    }
}
