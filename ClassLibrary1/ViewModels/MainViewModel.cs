using ManiaLabs.Helpers;
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
    public class MainViewModel : ViewModelBase
    {
        public DateTime CurrentTime => ManiaLabs.Platform.Get<IClock>().Now;

        public Models.Controller Controller => Models.Controller.Current as Models.Controller;

        public IList<Models.GenPeriod> Periods => Models.Schedule.Current.Periods;

        public ICommand StartCommand => new DelegateCommand(async _ => 
        {
            try
            {
                Models.Schedule.Current.Override();
                await Models.Controller.Current.Start();
            }
            catch (Exception ex)
            {
                base.SetError("MV1", ex);
            }
        });
        public ICommand StopCommand => new DelegateCommand(async _ =>
        {
            try
            {
                Models.Schedule.Current.Override();
                await Models.Controller.Current.Stop();
            }
            catch (Exception ex)
            {
                base.SetError("MV2", ex);
            }
        });


        public void Update()
        {
            try
            {
                base.SetProperty((nameof(CurrentTime)));
            }
            catch (Exception ex)
            {
                base.SetError("MV3", ex);
            }
        }
    }
}
