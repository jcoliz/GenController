using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    /// <summary>
    /// Controls the hardware interface to the generator
    /// </summary>
    /// <remarks>
    /// Hardware schematic:
    /// * Start relay
    /// * Stop relay
    /// * IsRunning input
    /// </remarks>
    public class Controller : IController, INotifyPropertyChanged
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
                ManiaLabs.Platform.Get<IMeasurement>().LogEvent(_Status.ToString());

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

            StopRelay = true;
            await Clock.Delay(StopPinHigh);
            StopRelay = false;
            await Clock.Delay(DelayBetweenStartAndStop);
            StartRelay = true;
            await Clock.Delay(StartPinHigh);
            StartRelay = false;

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

            StopRelay = true;
            await Clock.Delay(StopPinHigh);
            StopRelay = false;

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
                return Models.Schedule.Current.Enabled;
            }
            set
            {
                Models.Schedule.Current.Enabled = value;
                DoPropertyChanged(nameof(Enabled));
            }
        }

        public static IController Current
        {
            get
            {
                if (null == _Current)
                    _Current = new Controller();
                return _Current;
            }
            
            set
            {
                // Setting the controller is allowed for platform injection during tests
                _Current = value;
            }
        }
        static IController _Current = null;

        /// <summary>
        /// Get the current controller if exists, but don't make a new one yet.
        /// </summary>
        public static IController TryCurrent => _Current;

        #region Hardware Interface

        // See wiring and timing diagram here:
        // http://www.magnum-dimensions.com/sites/default/files/MagAGS/ME-AGS-Onan-Models-HGJAD-HGJAE-HGJAF-Rev-12-02-08.pdf

        private readonly TimeSpan StopPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan StartPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan DelayBetweenStartAndStop = TimeSpan.FromSeconds(4);
        private readonly TimeSpan DelayBetweenStartAttempts = TimeSpan.FromMinutes(2);
        private readonly TimeSpan DelayBetweenStartAndCheck = TimeSpan.FromSeconds(10);

        private IGenerator Generator = ManiaLabs.Platform.TryGet<IGenerator>();

        /// <summary>
        /// Controls the hardware relay pin to close the 'stop' line to the generator
        /// </summary>
        public bool StopRelay
        {
            get { return Generator?.StopOutput ?? false; }
            private set
            {
                if (null != Generator && value != Generator.StopOutput)
                {
                    Generator.StopOutput = value;
                    DoPropertyChanged(nameof(StopRelay));
                }
            }
        }

        /// <summary>
        /// Controls the hardware relay pin to close the 'start' line to the generator
        /// </summary>
        public bool StartRelay
        {
            get { return Generator?.StartOutput ?? false; }
            private set
            {
                if (null != Generator && value != Generator.StartOutput)
                {
                    Generator.StartOutput = value;
                    DoPropertyChanged(nameof(StartRelay));
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
        public double Voltage => Math.Floor((Generator?.Voltage ?? 0.0) * 10.0) / 10.0;

        /// <summary>
        /// Call this very frequently. This will debounce the runsignal line. It looks for
        /// 31 consecutive readings opposite the previous reading.
        /// </summary>
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

        private IClock Clock => ManiaLabs.Platform.TryGet<IClock>();
        private SynchronizationContext Context = SynchronizationContext.Current;

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
