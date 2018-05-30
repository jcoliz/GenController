using Commonality;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Provides a logical interface to the generator
    /// </summary>
    /// <remarks>
    /// The hardware interface to the generator is found in an IGenerator. 
    /// This should already be set in the service locator.
    /// However, if there is no IGenerator (perhaps because the app is running
    /// natively with no actual GPIO), this class will simply maintain
    /// what the status would be.
    /// 
    /// Service Dependencies:
    ///     * ILogger
    ///     * IClock
    ///     * IGenerator
    ///     * ISchedule
    /// 
    /// </remarks>
    public class Controller : IController, INotifyPropertyChanged, IVoltage
    {
        public GenStatus Status
        {
            get
            {
                return _Status;
            }
            private set
            {
                _Status = value;
                DoPropertyChanged(nameof(Status));
                Logger?.LogEvent(_Status.ToString());

                if (Generator != null)
                {
                    Generator.WarningLight = GenStatus.Initializing == value;
                    Generator.CommsLight = GenStatus.Starting == value || GenStatus.Stopping == value;
                }
                if (value == GenStatus.Initializing)
                {
                    StartedAt = null;
                }
            }
        }
        private GenStatus _Status = GenStatus.Initializing;

        public async Task Start()
        {
            // Already running? Don't do another stop/start cycle
            if (Status == GenStatus.Running && RunSignal)
                return;

            // Already processing a starting/stopping operation? Or disabled? Skip it
            if (Status == GenStatus.Starting || Status == GenStatus.Stopping || Status == GenStatus.Initializing)
                return;

            // If tehre is no clock yet, we can't do anything
            if (Clock == null)
                return;

            Status = GenStatus.Starting;

            StopLine = true;
            await Clock.Delay(StopPinHighDuration);
            StopLine = false;
            await Clock.Delay(DelayBetweenStartAndStop);
            StartLine = true;
            await Clock.Delay(StartPinHighDuration);
            StartLine = false;

            Status = GenStatus.Confirming;
        }
        public async Task Stop()
        {
            // Already processing a starting/stopping operation? Or Disabled? Skip it
            if (Status == GenStatus.Starting || Status == GenStatus.Stopping || Status == GenStatus.Initializing)
                return;

            // If tehre is no clock yet, we can't do anything
            if (Clock == null)
                return;

            Status = GenStatus.Stopping;

            StopLine = true;
            await Clock.Delay(StopPinHighDuration);
            StopLine = false;

            Status = GenStatus.Stopped;
        }
        /// <summary>
        /// Generator should be running, let's check on it
        /// </summary>
        public void Confirm()
        {
            if (Status == GenStatus.Confirming && RunSignal)
                Status = GenStatus.Running;
        }

        /// <summary>
        /// Whether the schedule is enabled
        /// </summary>
        /// <remarks>
        /// This is kept here for simplicity because all other system status is here.
        /// </remarks>
        public bool Enabled
        {
            get
            {
                return Schedule.Enabled;
            }
            set
            {
                Schedule.Enabled = value;
                DoPropertyChanged(nameof(Enabled));
            }
        }

        #region Hardware Interface

        // See wiring and timing diagram here:
        // http://www.magnum-dimensions.com/sites/default/files/MagAGS/ME-AGS-Onan-Models-HGJAD-HGJAE-HGJAF-Rev-12-02-08.pdf

        private readonly TimeSpan StopPinHighDuration = TimeSpan.FromSeconds(10);
        private readonly TimeSpan StartPinHighDuration = TimeSpan.FromSeconds(10);
        private readonly TimeSpan DelayBetweenStartAndStop = TimeSpan.FromSeconds(4);
        private readonly TimeSpan DelayBetweenStartAttempts = TimeSpan.FromMinutes(2);
        private readonly TimeSpan DelayBetweenStartAndCheck = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Controls the hardware output pin to close the 'stop' line to the generator
        /// </summary>
        public bool StopLine
        {
            get { return Generator?.StopOutput ?? false; }
            private set
            {
                if (null != Generator && value != Generator.StopOutput)
                {
                    Generator.StopOutput = value;
                    DoPropertyChanged(nameof(StopLine));
                }
            }
        }

        /// <summary>
        /// Controls the hardware output pin to close the 'start' line to the generator
        /// </summary>
        public bool StartLine
        {
            get { return Generator?.StartOutput ?? false; }
            private set
            {
                if (null != Generator && value != Generator.StartOutput)
                {
                    Generator.StartOutput = value;
                    DoPropertyChanged(nameof(StartLine));
                }
            }
        }

        /// <summary>
        /// The hardware input line coming from the generator indicating the generator
        /// is running
        /// </summary>
        public bool RunSignal => Generator?.RunInput ?? false;

        /// <summary>
        /// The hardware input line coming from the generator indicating the generator
        /// panel light should be lit
        /// </summary>
        public bool PanelLightSignal => Generator?.PanelLightInput ?? false;

        /// <summary>
        /// Current voltage on the "voltage sense" line
        /// </summary>
        /// <remarks>
        /// Rounded to the lowest tenth of a volt, for display purposes
        /// </remarks>
        public double Voltage => Math.Floor((Generator?.Voltage ?? 0.0) * 10.0) / 10.0;

        /// <summary>
        /// Call this very frequently. This will debounce the runsignal line. It looks for
        /// 31 consecutive readings opposite the previous reading.
        /// </summary>
        /// <remarks>
        /// This is a very conservative approach. While the underlying GPIO pin has debounce
        /// as well, we want a really CERTAIN indication that the run signal is changed.
        /// This smooths out variance which can exist in the hostile environment where the
        /// generator operates.
        /// </remarks>
        public void FastTick()
        {
            RunSignalBits <<= 1;
            RunSignalBits |= (RunSignal ? 1u : 0);

            if (RunSignalBits == SignalOffEdge || RunSignalBits == SignalOnEdge)
                DoPropertyChanged(nameof(RunSignal));

            PanelLightSignalBits <<= 1;
            PanelLightSignalBits |= (PanelLightSignal ? 1u : 0);

            if (PanelLightSignalBits == SignalOffEdge || PanelLightSignalBits == SignalOnEdge)
                DoPropertyChanged(nameof(PanelLightSignal));

            if (Status == GenStatus.Initializing)
            {
                if (Clock != null)
                {
                    if (!StartedAt.HasValue)
                    {
                        // WARNING: Don't change the clock while initializing!!
                        StartedAt = Clock.Now;
                    }
                    else if (Clock.Now - StartedAt.Value > TimeSpan.FromSeconds(30))
                    {
                        Status = RunSignal ? GenStatus.Running : GenStatus.Stopped;
                    }
                }
            }
        }

        /// <summary>
        /// Call as often as you update the UI (every second is good)
        /// </summary>
        public void SlowTick()
        {
            DoPropertyChanged(nameof(Voltage));
        }

        private UInt32 RunSignalBits = 0;
        private UInt32 PanelLightSignalBits = 0;
        private const UInt32 SignalOffEdge = 1u << 31;
        private const UInt32 SignalOnEdge = UInt32.MaxValue >> 1;
        private DateTime? StartedAt;
        #endregion

        private SynchronizationContext Context = SynchronizationContext.Current;

        #region Service Locator services

        private ILogger Logger => Service.TryGet<ILogger>();
        private IClock Clock => Service.TryGet<IClock>();
        private IGenerator Generator => Service.TryGet<IGenerator>();
        private ISchedule Schedule => Service.Get<ISchedule>();

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void DoPropertyChanged(string name)
        {
            if (Context != null)
                Context.Post((o) => 
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }, null);
            else
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public enum GenStatus { Invalid = 0, Stopped, Starting, Confirming, Running, Stopping, Initializing };
}
