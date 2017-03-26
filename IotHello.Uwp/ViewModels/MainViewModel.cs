using ManiaLabs.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentTime { get; set; }

        public string Status => Models.GenController.Current.FullStatus;

        public ObservableCollection<Models.GenPeriod> Periods = new ObservableCollection<Models.GenPeriod>()
        {
            new Models.GenPeriod(TimeSpan.FromHours(7),TimeSpan.FromHours(9)),
            new Models.GenPeriod(TimeSpan.FromHours(12),TimeSpan.FromHours(14)),
            new Models.GenPeriod(TimeSpan.FromHours(17),TimeSpan.FromHours(19))
        };

        public Models.Action On =
            new Models.Action() { Label = "On", Command = new DelegateCommand(TurnOn), Color = "Green" };

        public Models.Action Off =
            new Models.Action() { Label = "Off", Command = new DelegateCommand(TurnOff), Color = "Red" };

        private static async void TurnOn(object obj)
        {
            await Models.GenController.Current.Start();
        }

        private static async void TurnOff(object obj)
        {
            await Models.GenController.Current.Stop();
        }

        public void Update()
        {
            try
            {
                CurrentTime = DateTime.Now.ToString("H\\:mm\\:ss");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
