using ManiaLabs.Helpers;
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
        DateTime DT { get; set; }

        Portable.ViewModels.MainViewModel VM = new Portable.ViewModels.MainViewModel();

        public ICommand AddCommand => new DelegateCommand((x) => 
        {
            string what = x as string;
            char direction = what[0];
            int add = 1;
            if (direction == '-')
                add = -1;
            switch(what.Substring(1))
            {
                case "year":
                    DT = DT.AddYears(add);
                    break;
                case "month":
                    DT = DT.AddMonths(add);
                    break;
                case "day":
                    DT = DT.AddDays(add);
                    break;
                case "hour":
                    DT = DT.AddHours(add);
                    break;
                case "minute":
                    DT = DT.AddMinutes(add);
                    break;
                case "second":
                    DT = DT.AddSeconds(add);
                    break;
            }

            Bindings.Update();
        });

        public Settings()
        {
            this.InitializeComponent();

            var clock = ManiaLabs.Platform.Get<Portable.Models.IClock>();
            DT = clock.Now;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
