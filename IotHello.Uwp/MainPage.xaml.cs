using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IotHello.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ViewModels.MainViewModel VM = new ViewModels.MainViewModel();
        private DispatcherTimer Timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

        public MainPage()
        {
            this.InitializeComponent();
            Timer.Tick += (s,e) => { VM.Update(); };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            VM.Update();
            Timer.Start();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Timer.Stop();
            base.OnNavigatedFrom(e);
        }
    }
}
