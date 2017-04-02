using ManiaLabs.Helpers;
using ManiaLabs.Models;
using ManiaLabs.Portable.Base;
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
        public RangeObservableCollection<Models.GenPeriod> Periods = new RangeObservableCollection<Models.GenPeriod>();

        /// <summary>
        /// Dependency injection for how to get the current time.
        /// </summary>
        private IClock Clock => ManiaLabs.Platform.Get<IClock>();

        /// <summary>
        /// Load schedule from storage
        /// </summary>
        public void Load()
        {
            var storage = Setting.GetCompositeKey("Schedule");
            Periods.AddRange(storage.Select(GenPeriod.Deserialize));
        }

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
                        Log("Schedule.RetryStart");
                        StartedConfirmingAt = null;
                        result = Controller.Current.Start();
                    }
                }
            }
            else if (StartedConfirmingAt != null)
            {
                Log("Schedule.ConfirmedStart");
                StartedConfirmingAt = null;
            }

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
                            Log("Schedule.Start");
                            result = Controller.Current.Start();
                            break;
                        }
                        if (LastTick < stopat && now >= stopat)
                        {
                            Log("Schedule.Stop");
                            result = Controller.Current.Stop();
                            break;
                        }
                    }
                }
            }
            LastTick = now;

            return result;
        }

        /// <summary>
        /// Replaces the given old scehdule with the provided replacement
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Thrown if replacing this would cause overlapping schedules
        /// </exception>
        /// <param name="old"></param>
        /// <param name="replacement"></param>
        public void Replace(GenPeriod old, GenPeriod replacement)
        {
            if (replacement.StartAt < TimeSpan.Zero || replacement.StartAt >= TimeSpan.FromDays(1) ||
                replacement.StopAt < TimeSpan.Zero || replacement.StartAt >= TimeSpan.FromDays(1))
            {
                throw new ArgumentException("Invalid start or stop time", nameof(replacement));
            }

            if (replacement.StopAt - replacement.StartAt < TimeSpan.FromMinutes(30))
                throw new ArgumentException("Generator must run at least 30 minutes", nameof(replacement));

            var proposed = Periods.ToList();
            proposed.Remove(old);
            proposed.Add(replacement);
            proposed.Sort();

            TimeSpan laststop = TimeSpan.FromDays(-1);
            foreach(var p in proposed)
            {
                if (p.StartAt < laststop)
                    throw new ArgumentException("Replacement period overlaps an existing period", nameof(replacement));
                if (p.StartAt - laststop < TimeSpan.FromMinutes(30))
                    throw new ArgumentException("Generator must wait 30 minutes before starting again", nameof(replacement));
                laststop = p.StopAt;
            }

            Periods.Clear();
            Periods.AddRange(proposed);

            Setting.SetCompositeKey("Schedule",Periods.Select(GenPeriod.Serialize));
        }

        public void Remove(GenPeriod old)
        {
            Periods.Remove(old);
            Setting.SetCompositeKey("Schedule", Periods.Select(GenPeriod.Serialize));
        }

        private DateTime LastTick = DateTime.MinValue;
        private DateTime? StartedConfirmingAt = null;

        private void Log(string what)
        {
            var Measurement = ManiaLabs.Platform.TryGet<IMeasurement>();
            Measurement?.LogEvent(what);
        }

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
