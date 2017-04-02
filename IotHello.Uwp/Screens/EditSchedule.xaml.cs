using ManiaLabs.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

namespace IotHello.Uwp.Screens
{
    public sealed partial class EditSchedule : Page
    {

        Portable.ViewModels.EditScheduleViewModel VM = new Portable.ViewModels.EditScheduleViewModel();
        Portable.ViewModels.MainViewModel MainVM = new Portable.ViewModels.MainViewModel();

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
            base.OnNavigatedTo(e);
        }
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private async void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VM.Commit();
                Frame.GoBack();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
            }
        }
    }
}
