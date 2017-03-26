﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullStatus)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
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

        public async Task Start()
        {
            Status = GenStatus.Starting;

            StopRelay = true;
            await Task.Delay(StopPinHigh);
            StopRelay = false;
            await Task.Delay(DelayBetweenStartAndStop);
            StartRelay = true;
            await Task.Delay(StartPinHigh);
            StartRelay = false;
            await Task.Delay(DelayBetweenStartAndCheck);

            // TODO: This could be brittle, only checking the run signal ONCE.
            // Instead, we should check it every tick during the delay time.
            // If it comes on even once during those checks, we are running!!
            if (RunSignal)
                Status = GenStatus.Running;
            else
                Status = GenStatus.FailedToStart;
        }
        public async Task Stop()
        {
            Status = GenStatus.Stopping;

            StopRelay = true;
            await Task.Delay(StopPinHigh);
            StopRelay = false;

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
        private bool StopRelay
        {
            set
            {
                _StopRelay = value;

                // Temporary, until there is really a run signal hooked up
                if (value)
                    RunSignal = false;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullStatus)));
            }
        }
        private bool _StopRelay = false;

        /// <summary>
        /// Controls the hardware relay pin to close the 'start' line to the generator
        /// </summary>
        private bool StartRelay
        {
            set
            {
                _StartRelay = value;

                // Temporary, until there is really a run signal hooked up
                if (value)
                    RunSignal = true;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullStatus)));
            }
        }
        private bool _StartRelay = false;

        /// <summary>
        /// The hardware input line coming from the generator.
        /// </summary>
        /// <remarks>
        /// For now, this is stubbed out to assume all starts always work.
        /// </remarks>
        private bool RunSignal { get; set; }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public enum GenStatus { Invalid = 0, Stopped, Starting, Running, Stopping, FailedToStart };


}
