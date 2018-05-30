using Commonality;
using System;
using System.Windows.Input;

namespace GenController.Portable.ViewModels
{
    /// <summary>
    /// View model for use with the settings screen
    /// </summary>
    /// <remarks>
    /// Service Dependencies:
    ///     * IClock
    ///     * ILogger
    /// </remarks>
    public class SettingsViewModel: ViewModelBase
    {
        public SettingsViewModel(): base(Service.TryGet<ILogger>())
        {
        }

        /// <summary>
        /// DateTime currently being edited
        /// </summary>
        public DateTime DT
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
        private TimeSpan Delta = TimeSpan.Zero;

        /// <summary>
        /// Add time to the DT
        /// </summary>
        public ICommand AddCommand => new DelegateCommand((x) =>
        {
            try
            {
                string what = x as string;
                char direction = what[0];
                int add = 1;
                if (direction == '-')
                    add = -1;
                switch (what.Substring(1))
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

                base.SetProperty(nameof(DT));
            }
            catch (Exception ex)
            {
                base.SetError("SV1", ex);
            }
        });

        public void Commit()
        {
            try
            {
                Clock.Now = DT;
                DT = Clock.Now;
            }
            catch (Exception ex)
            {
                base.SetError("SV2", ex);
            }
        }

        /// <summary>
        /// Service Locator for the clock
        /// </summary>
        private IClock Clock => Service.TryGet<IClock>();
    }
}
