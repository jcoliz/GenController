﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Models
{
    public class Schedule
    {
        public ObservableCollection<Models.GenPeriod> Periods = new ObservableCollection<Models.GenPeriod>()
        {
            new Models.GenPeriod(TimeSpan.FromHours(7),TimeSpan.FromHours(9)),
            new Models.GenPeriod(TimeSpan.FromHours(12),TimeSpan.FromHours(14)),
            new Models.GenPeriod(TimeSpan.FromHours(17),TimeSpan.FromHours(19))
        };

        /// <summary>
        /// Call this regularly to test the schedule to see if it's time to change
        /// state.
        /// </summary>
        public async Task Tick()
        {
            var now = DateTime.Now;
            var elapsed = now - LastTick;

            // Don't take action if we are currently TRYING to start or stop
            if (Controller.Current.Status != GenStatus.Starting && Controller.Current.Status != GenStatus.Stopping)
            {
                // TODO: Deal with failed starts. This will require getting the input line from the
                // generator itself.

                // Only take action if we've been called recently
                if (elapsed < TimeSpan.FromSeconds(5))
                {
                    // Check every edge
                    foreach (var p in Periods)
                    {
                        var today = now.Date;
                        DateTime startat = today + p.StartAt;
                        DateTime stopat = today + p.StopAt;

                        if (LastTick < startat && now >= startat)
                        {
                            await Controller.Current.Start();
                            break;
                        }
                        if (LastTick < stopat && now >= stopat)
                        {
                            await Controller.Current.Stop();
                            break;
                        }
                    }
                }
            }
            LastTick = now;
        }

        private DateTime LastTick = DateTime.MinValue;

        public static Schedule Current
        {
            get
            {
                if (null == _Current)
                    _Current = new Schedule();
                return _Current;
            }
        }
        static Schedule _Current = null;
    }
}
