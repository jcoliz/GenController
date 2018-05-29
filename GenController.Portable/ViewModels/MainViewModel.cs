using System;
using System.Collections.Generic;
using System.Windows.Input;
using Commonality;
using GenController.Portable.Models;

namespace GenController.Portable.ViewModels
{
    /// <summary>
    /// Viewmodel for the main screen
    /// </summary>
    /// <remarks>
    /// Service Dependencies:
    ///     * IClock
    ///     * ISchedule
    /// </remarks>
    public class MainViewModel : ViewModelBase
    {
        public DateTime CurrentTime => Clock?.Now ?? DateTime.MinValue;

        public Models.Controller Controller => Models.Controller.Current as Models.Controller;

        public ICollection<Models.GenPeriod> Periods => Schedule.Periods;

        public ICommand StartCommand => new DelegateCommand(async _ => 
        {
            try
            {
                Schedule.Override();
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
                Schedule.Override();
                await Models.Controller.Current.Stop();
            }
            catch (Exception ex)
            {
                base.SetError("MV2", ex);
            }
        });

        public ICommand EnableCommand => new DelegateCommand(_ =>
        {
            try
            {
                Controller.Enabled = true;
            }
            catch (Exception ex)
            {
                base.SetError("MV4", ex);
            }
        });

        public ICommand DisableCommand => new DelegateCommand(_ =>
        {
            try
            {
                Controller.Enabled = false;
            }
            catch (Exception ex)
            {
                base.SetError("MV5", ex);
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

        private IClock Clock => Service.TryGet<IClock>();
        private ISchedule Schedule => Service.Get<ISchedule>();
    }
}
