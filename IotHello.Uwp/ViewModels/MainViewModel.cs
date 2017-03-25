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

        public string CurrentTime { get; set; } = "12:34:56";

        public ObservableCollection<string> Infos = new ObservableCollection<string>() { $"0700{Environment.NewLine}0900", $"1200{Environment.NewLine}1400", $"1700{Environment.NewLine}1900", "+Add" };

        public ObservableCollection<Models.Action> Actions = new ObservableCollection<Models.Action>()
        {
            new Models.Action() { Label = "On", Command = new DelegateCommand(TurnOn) },
            new Models.Action() { Label = "Off", Command = new DelegateCommand(TurnOff) }
        };

        private static void TurnOn(object obj)
        {
        }

        private static void TurnOff(object obj)
        {
        }

        public void Update()
        {
        }
    }
}
