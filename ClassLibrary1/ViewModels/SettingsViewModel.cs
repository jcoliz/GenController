﻿using Common.Portable.Helpers;
using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IotHello.Portable.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
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
        });

        public void Commit()
        {
            Clock.Now = DT;
            DT = Clock.Now;
        }

        /// <summary>
        /// Service Locator for the clock
        /// </summary>
        private IClock Clock => ManiaLabs.Platform.Get<IClock>();
    }
}