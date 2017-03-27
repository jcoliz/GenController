using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    public class Schedule
    {
        public ObservableCollection<Models.GenPeriod> Periods = new ObservableCollection<Models.GenPeriod>();

        /// <summary>
        /// Dependency injection for how to get the current time.
        /// </summary>
        public IClock Clock { get; set; }

        /// <summary>
        /// Call this regularly to test the schedule to see if it's time to change
        /// state.
        /// </summary>
        /// <remarks>
        /// Whether something changed
        /// </remarks>
        public Task Tick()
        {
            Task result = null;
            var now = Clock.Now;
            var elapsed = now - LastTick;
            var status = Controller.Current.Status;

            // Managed failed starts
            if (status == GenStatus.Confirming)
            {
                // Confirm whether the generator is running now
                Controller.Current.Confirm();

                // If this is the first we've heard about it, start the timer
                if (StartedConfirmingAt == null)
                    StartedConfirmingAt = now;
                else
                {
                    var elapsedfailed = now - StartedConfirmingAt;
                    if (elapsedfailed > TimeSpan.FromMinutes(2))
                    {
                        // Try again!!
                        StartedConfirmingAt = null;
                        result = Controller.Current.Start();
                    }
                }
            }
            else if (StartedConfirmingAt != null)
                StartedConfirmingAt = null;

            // Don't take action if we are currently TRYING to start or stop
            if (status != GenStatus.Starting && status != GenStatus.Stopping && status != GenStatus.Confirming)
            {
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
                            result = Controller.Current.Start();
                            break;
                        }
                        if (LastTick < stopat && now >= stopat)
                        {
                            result = Controller.Current.Stop();
                            break;
                        }
                    }
                }
            }
            LastTick = now;

            return result;
        }

        private DateTime LastTick = DateTime.MinValue;
        private DateTime? StartedConfirmingAt = null;

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
