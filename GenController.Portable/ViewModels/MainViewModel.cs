using System;
using System.Collections.Generic;
using System.Windows.Input;
using Commonality;

namespace GenController.Portable.ViewModels
{
    /// <summary>
    /// Viewmodel for the main screen
    /// </summary>
    /// <remarks>
    /// Service Dependencies:
    ///     * IClock
    /// </remarks>
    public class MainViewModel : ViewModelBase
    {
        public DateTime CurrentTime => Clock?.Now ?? DateTime.MinValue;

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
    }
}
