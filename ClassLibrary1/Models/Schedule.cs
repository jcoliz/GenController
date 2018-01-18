using Setting = ManiaLabs.Models.Setting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    /// <summary>
    /// The overall schedule of the generator, containing all the schedule blocks
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// The actual schedule blocks
        /// </summary>
        public RangeObservableCollection<Models.GenPeriod> Periods = new RangeObservableCollection<Models.GenPeriod>();

        /// <summary>
        /// Schedule is free to start and stop generator according to schedule
        /// </summary>
        public bool Enabled
        {
            get
            {
                return Boolean.Parse(Setting.GetKeyValueWithDefault("Enabled", "True"));
            }
            set
            {
                Setting.SetKey("Enabled", value.ToString());
            }
        }
        //Setting.SetCompositeKey("Schedule",Periods.Select(GenPeriod.Serialize));

        public Schedule()
        {
            Periods.CollectionChanged += Periods_CollectionChanged;
        }

        private void Periods_CollectionChanged(object sender = null, System.Collections.Specialized.NotifyCollectionChangedEventArgs e = null)
        {
            // Reorganize the schedule into a format that's easy for the scheduler to quickly determine what should be
            // happening right now
            InternalSchedule.Clear();
            if (Periods.FirstOrDefault() != null)
            {
                InternalSchedule.AddRange(Periods.Select(x => new ScheduleItem() { Time = x.StartAt, DesiredState = GenStatus.Running, Voltage = x.Voltage }));
                InternalSchedule.AddRange(Periods.Select(x => new ScheduleItem() { Time = x.StopAt, DesiredState = GenStatus.Stopped }));

                if (_Override != null)
                    InternalSchedule.Add(_Override);

                InternalSchedule.Sort();

                // Add tomorrow's first at the end, and yesterday's last at the start
                var first = InternalSchedule.First();
                var last = InternalSchedule.Last();
                InternalSchedule.Add(new ScheduleItem() { Time = first.Time + TimeSpan.FromDays(1), DesiredState = first.DesiredState, Voltage = first.Voltage });
                InternalSchedule.Insert(0, new ScheduleItem() { Time = last.Time - TimeSpan.FromDays(1), DesiredState = last.DesiredState, Voltage = last.Voltage });
            }
        }

        /// <summary>
        /// Load schedule from storage
        /// </summary>
        public void Load()
        {
            var storage = Setting.GetCompositeKey("Schedule");
            Periods.AddRange(storage.Select(GenPeriod.Deserialize));
            Measurement.LogEvent("Schedule.Loaded", $"Schedule={string.Join(",",storage)}");
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
            // If tehre is no clock yet, we can't do anything
            if (Clock == null)
                return null;

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
            if (status != GenStatus.Starting && status != GenStatus.Stopping && status != GenStatus.Confirming && status != GenStatus.Initializing && Enabled)
            {
                // First, figure out what should be happening right now.
                var found = InternalSchedule.BinarySearch(new ScheduleItem() { Time = now.TimeOfDay });
                if (found < 0)
                    found = (~found) - 1;

                var item = InternalSchedule[found];
                var desiredstate = item.DesiredState;

                // If the desired state is 'invalid', then we are in a schedule override mode, so we are ignoring
                // the schedule as long as that lasts.
                if (desiredstate != status && desiredstate != GenStatus.Invalid)
                {
                    if (_Override != null)
                    {
                        _Override = null;
                        Periods_CollectionChanged();
                    }

                    // Take action!!
                    if (desiredstate == GenStatus.Running)
                    {
                        // If we want to start, let's check the voltage levels to understand what we need
                        if (item.Voltage < 1.0 || Controller.Current.Voltage < item.Voltage)
                        {
                            Log("Schedule.Start");
                            result = Controller.Current.Start();
                        }
                    }
                    else if (desiredstate == GenStatus.Stopped)
                    {
                        Log("Schedule.Stop");
                        result = Controller.Current.Stop();
                    }
                }
            }
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
            if (old != null)
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

        /// <summary>
        /// Add this block to the schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(GenPeriod item) => Replace(null, item);

        /// <summary>
        /// Override the schedule until the next scheduled start time
        /// </summary>
        public void Override()
        {
            // If tehre is no clock yet, we can't do anything
            if (Clock == null)
                return;

            _Override = new ScheduleItem() { Time = Clock.Now.TimeOfDay, DesiredState = GenStatus.Invalid };
            Periods_CollectionChanged();
        }

        private ScheduleItem _Override;

        /// <summary>
        /// Remove this block from the schedule
        /// </summary>
        /// <param name="old"></param>
        public void Remove(GenPeriod old)
        {
            Periods.Remove(old);
            Setting.SetCompositeKey("Schedule", Periods.Select(GenPeriod.Serialize));
        }

        /// <summary>
        /// The current singleton
        /// </summary>
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

        private DateTime LastTick = DateTime.MinValue;
        private DateTime? StartedConfirmingAt = null;

        private void Log(string what) =>  Measurement?.LogEvent(what);

        /// <summary>
        /// Service Locator for how to get the current time.
        /// </summary>
        private IClock Clock => ManiaLabs.Platform.TryGet<IClock>();

        /// <summary>
        /// Service Locator for how to get measurement and logging.
        /// </summary>
        private IMeasurement Measurement => ManiaLabs.Platform.TryGet<IMeasurement>();

        class ScheduleItem : IComparable<ScheduleItem>, IComparable<TimeSpan>
        {
            public TimeSpan Time;
            public GenStatus DesiredState;
            public double Voltage;

            public int CompareTo(ScheduleItem other) => Time.CompareTo(other.Time);
            public int CompareTo(TimeSpan other) => Time.CompareTo(other);
        }

        /// <summary>
        /// This is a decomposed copy of the schedule periods. It is stored in a format making it
        /// easy to test.
        /// </summary>
        private List<ScheduleItem> InternalSchedule = new List<ScheduleItem>();
    }
}
