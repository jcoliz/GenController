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
                DoPropertyChanged(nameof(FullStatus));
                DoPropertyChanged(nameof(Status));
                ManiaLabs.Platform.Get<IMeasurement>().LogEvent(_Status.ToString());
            }
        }
        private GenStatus _Status = GenStatus.Stopped;

        public string FullStatus
        {
            get
            {
                string result = Status.ToString();

                if (_StartRelay)
                    result += " P1";

                if (_StopRelay)
                    result += " P2";

                return result;
            }
        }

        private SynchronizationContext Context = SynchronizationContext.Current; 

        public async Task Start()
        {
            // Already running? Don't do another stop/start cycle
            if (Status == GenStatus.Running && RunSignal)
                return;

            // Already processing a starting/stopping operation? Or disabled? Skip it
            if (Status == GenStatus.Starting || Status == GenStatus.Stopping || Status == GenStatus.Disabled)
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
            if (Status == GenStatus.Starting || Status == GenStatus.Stopping || Status == GenStatus.Disabled)
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

        public void ToggleDisable()
        {
            if (Status == GenStatus.Stopped)
                Status = GenStatus.Disabled;
            else if (Status == GenStatus.Disabled)
                Status = GenStatus.Stopped;
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

        #region Hardware Interface

        // See wiring and timing diagram here:
        // http://www.magnum-dimensions.com/sites/default/files/MagAGS/ME-AGS-Onan-Models-HGJAD-HGJAE-HGJAF-Rev-12-02-08.pdf

        private readonly TimeSpan StopPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan StartPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan DelayBetweenStartAndStop = TimeSpan.FromSeconds(4);
        private readonly TimeSpan DelayBetweenStartAttempts = TimeSpan.FromMinutes(2);
        private readonly TimeSpan DelayBetweenStartAndCheck = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Controls the hardware relay pin to close the 'stop' line to the generator
        /// </summary>
        public bool StopRelay
        {
            get { return _StopRelay; }
            private set
            {
                if (value != _StopRelay)
                {
                    _StopRelay = value;

                    // Temporary, until there is really a run signal hooked up
                    if (value)
                        RunSignal = false;

                    DoPropertyChanged(nameof(FullStatus));
                    DoPropertyChanged(nameof(StopRelay));
                }
            }
        }
        private bool _StopRelay = false;

        /// <summary>
        /// Controls the hardware relay pin to close the 'start' line to the generator
        /// </summary>
        public bool StartRelay
        {
            get { return _StartRelay; }
            private set
            {
                if (value != _StartRelay)
                {
                    _StartRelay = value;

                    // Temporary, until there is really a run signal hooked up
                    if (value)
                        RunSignal = true;

                    DoPropertyChanged(nameof(FullStatus));
                    DoPropertyChanged(nameof(StartRelay));
                }
            }
        }
        private bool _StartRelay = false;

        /// <summary>
        /// The hardware input line coming from the generator.
        /// </summary>
        /// <remarks>
        /// For now, this is stubbed out to assume all starts always work.
        /// TODO: Probably should filter this runsignal, checking its value over time, so I don't
        /// overreact to one bad reading.
        /// </remarks>
        public bool RunSignal
        {
            get { return _RunSignal; }
            private set
            {
                if (_RunSignal != value)
                {
                    _RunSignal = value;
                    DoPropertyChanged(nameof(RunSignal));
                }
            }
        }
        private bool _RunSignal = false;

        /// <summary>
        /// Call this very frequently. This will debounce the runsignal line. It looks for
        /// 31 consecutive readings opposite the previous reading.
        /// </summary>
        public void HardwareTick()
        {
            RunSignalBits <<= 1;
            RunSignalBits |= RunSignalLine;

            if (RunSignalBits == RunSignalOffEdge)
                RunSignal = false;
            else if (RunSignalBits == RunSignalOnEdge)
                RunSignal = true;
        }

        private UInt32 RunSignalBits = 0;
        private const UInt32 RunSignalOffEdge = 1u << 31;
        private const UInt32 RunSignalOnEdge = UInt32.MaxValue >> 1;

        /// <summary>
        /// The actual GPIO line coming from the generator run signal. Only valid values are
        /// 1 and 0
        /// </summary>
        private byte RunSignalLine => 0;
        #endregion

        private IClock Clock => ManiaLabs.Platform.Get<IClock>();

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

    public enum GenStatus { Invalid = 0, Stopped, Starting, Confirming, Running, Stopping, Disabled };
}
