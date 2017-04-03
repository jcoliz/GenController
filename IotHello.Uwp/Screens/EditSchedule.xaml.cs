﻿using System;
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
            App.Current.Measurement.LogEvent("Screen.EditSchedule", $"Add={VM.WillAdd}");
            if (e.Parameter != null && e.Parameter is Portable.Models.GenPeriod)
            {
                VM.Original = e.Parameter as Portable.Models.GenPeriod;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VM.ExceptionRaised -= VM_ExceptionRaised;
            base.OnNavigatedFrom(e);
        }

        private void VM_ExceptionRaised(object sender, ManiaLabs.Portable.Base.ExceptionArgs e)
        {
            string message = e.ex.Message;
            if (!string.IsNullOrEmpty(e.code))
                message += $" (Code {e.code})";
            var ignore = new MessageDialog(message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
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
