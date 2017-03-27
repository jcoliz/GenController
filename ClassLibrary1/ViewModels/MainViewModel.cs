using Common.Portable.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IotHello.Portable.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime CurrentTime => DateTime.Now;

        public Models.Controller Controller => Models.Controller.Current as Models.Controller;

        public ObservableCollection<Models.GenPeriod> Periods => Models.Schedule.Current.Periods;

        public ICommand StartCommand => new DelegateCommand(async _ => 
        {
            try
            {
                await Models.Controller.Current.Start();
            }
            catch (Exception)
            {
            }
        });
        public ICommand StopCommand => new DelegateCommand(async _ =>
        {
            try
            {
                await Models.Controller.Current.Stop();
            }
            catch (Exception)
            {
            }
        });


        public void Update()
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
            }
            catch (Exception)
            {
            }
        }
    }
}
