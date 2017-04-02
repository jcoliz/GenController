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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IotHello.Uwp.Screens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditSchedule : Page, INotifyPropertyChanged
    {
        public Portable.Models.GenPeriod Period { get; private set; }

        private Portable.Models.GenPeriod Original;

        Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool WillDelete { get; set; }

        public ICommand AddCommand => new DelegateCommand((x) =>
        {
            string what = x as string;
            char direction = what[0];
            int add = 1;
            if (direction == '-')
                add = -1;
            switch (what.Substring(1))
            {
                case "Hfrom":
                    Period.StartAt = Period.StartAt.Add(TimeSpan.FromHours(add));
                    if (Period.StartAt < TimeSpan.Zero)
                    {
                        Period.StartAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StartAt >= TimeSpan.FromDays(1))
                    {
                        Period.StartAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StopAt = Period.StartAt;
                    }
                    break;
                case "Mfrom":
                    Period.StartAt = Period.StartAt.Add(TimeSpan.FromMinutes(add * 5));
                    if (Period.StartAt < TimeSpan.Zero)
                    {
                        Period.StartAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StartAt >= TimeSpan.FromDays(1))
                    {
                        Period.StartAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StopAt = Period.StartAt;
                    }
                    break;
                case "Hto":
                    Period.StopAt = Period.StopAt.Add(TimeSpan.FromHours(add));
                    if (Period.StopAt < TimeSpan.Zero)
                    {
                        Period.StopAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StopAt >= TimeSpan.FromDays(1))
                    {
                        Period.StopAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StartAt = Period.StopAt;
                    }
                    break;
                case "Mto":
                    Period.StopAt = Period.StopAt.Add(TimeSpan.FromMinutes(add * 5));
                    if (Period.StopAt < TimeSpan.Zero)
                    {
                        Period.StopAt += TimeSpan.FromDays(1);
                    }
                    else if (Period.StopAt >= TimeSpan.FromDays(1))
                    {
                        Period.StopAt -= TimeSpan.FromDays(1);
                    }
                    if (Period.StartAt > Period.StopAt)
                    {
                        Period.StartAt = Period.StopAt;
                    }
                    break;
           }

            Bindings.Update();
        });

        public EditSchedule()
        {
            Period = new Portable.Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(13));
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is Portable.Models.GenPeriod)
            {
                Original = e.Parameter as Portable.Models.GenPeriod;
                Period = new Portable.Models.GenPeriod(Original.StartAt, Original.StopAt);
                Bindings.Update();
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
                if (WillDelete)
                    Portable.Models.Schedule.Current.Remove(Original);
                else
                    Portable.Models.Schedule.Current.Replace(Original, Period);

                Frame.GoBack();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, App.Current.GetResourceString("Sorry/Text").ToUpper()).ShowAsync();
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            WillDelete = !WillDelete;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WillDelete)));

            
        }
    }
}
