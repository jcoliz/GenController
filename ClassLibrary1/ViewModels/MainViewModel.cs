using Common.Portable.Helpers;
using ManiaLabs.Portable.Base;
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

        public DateTime CurrentTime => ManiaLabs.Platform.Get<Models.IClock>().Now;

        public Models.Controller Controller => Models.Controller.Current as Models.Controller;

        public ObservableCollection<Models.GenPeriod> Periods => Models.Schedule.Current.Periods;

        public string App
        {
            get
            {
                var app = ManiaLabs.Platform.Get<ManiaLabs.IApp>();
                return $"{app.Title} {app.Version}";
            }
        }

        public ICommand StartCommand => new DelegateCommand(async _ => 
        {
            try
            {
                await Models.Controller.Current.Start();
            }
            catch (Exception ex)
            {
                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("MV1", ex);
            }
        });
        public ICommand StopCommand => new DelegateCommand(async _ =>
        {
            try
            {
                await Models.Controller.Current.Stop();
            }
            catch (Exception ex)
            {
                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("MV2", ex);
            }
        });


        public void Update()
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTime)));
            }
            catch (Exception ex)
            {
                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("MV3", ex);
            }
        }
    }
}
