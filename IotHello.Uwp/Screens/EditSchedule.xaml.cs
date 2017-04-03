using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IotHello.Uwp.Screens
{
    public sealed partial class EditSchedule : Page
    {

        Portable.ViewModels.EditScheduleViewModel VM = new Portable.ViewModels.EditScheduleViewModel();

        private string Purpose => VM.WillAdd ? "Add Schedule Block" : "Edit Schedule";

        public EditSchedule()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Portable.Models.GenPeriod)
            {
                VM.Original = e.Parameter as Portable.Models.GenPeriod;
            }
            App.Current.Measurement.LogEvent("Screen.EditSchedule",$"Add={VM.WillAdd}");
            base.OnNavigatedTo(e);
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
                App.Current.Measurement.LogEvent("Schedule.Commit", $"Start={VM.Period.SerializeKey}",$"Delete={VM.WillDelete}");
                Frame.GoBack();
            }
            catch (Exception ex)
            {
                App.Current.Measurement.LogEvent("Schedule.CommitFailed",$"Reason={ex.Message}");
                var ignore = new MessageDialog(ex.Message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
            }
        }
    }
}
