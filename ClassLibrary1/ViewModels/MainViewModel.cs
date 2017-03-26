using Common.Portable.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentTime { get; set; }

        public Models.Controller Controller => Models.Controller.Current as Models.Controller;

        public ObservableCollection<Models.GenPeriod> Periods => Models.Schedule.Current.Periods;

        public Models.Action On =
            new Models.Action() { Label = "On", Command = new DelegateCommand(TurnOn), Color = "Green" };

        public Models.Action Off =
            new Models.Action() { Label = "Off", Command = new DelegateCommand(TurnOff), Color = "Red" };

        private static async void TurnOn(object obj)
        {
            await Models.Controller.Current.Start();
        }

        private static async void TurnOff(object obj)
        {
            await Models.Controller.Current.Stop();
        }

        public void Update()
        {
            try
            {
                CurrentTime = DateTime.Now.ToString("H\\:mm\\:ss");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));

                Models.Schedule.Current.Tick();
            }
            catch (Exception)
            {

            }
        }
    }
}
