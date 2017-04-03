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
        /// <summary>
        /// DateTime currently being edited
        /// </summary>
        DateTime DT
        {
            get
            {
                return Clock.Now + Delta;
            }
            set
            {
                Delta = value - Clock.Now;
            }
        }

        /// <summary>
        /// How much we have changed the time since we started
        /// </summary>
        TimeSpan Delta;

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

            Delta = TimeSpan.Zero;           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        private IClock Clock => ManiaLabs.Platform.Get<IClock>();

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Clock.Now = DT;
            Frame.GoBack();
        }
    }
}
