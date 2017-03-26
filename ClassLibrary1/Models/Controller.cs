using System;
using System.Collections.Generic;
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
    public class Controller : IController
    {
        public GenStatus Status { get; private set; } = GenStatus.Stopped;

        public string FullStatus
        {
            get
            {
                string result = Status.ToString();

                if (StartPin)
                    result += " P1";

                if (StopPin)
                    result += " P2";

                return result;
            }
        }

        public async Task Start()
        {
            Status = GenStatus.Starting;

            StopPin = true;
            await Task.Delay(StopPinHigh);
            StopPin = false;
            await Task.Delay(DelayBetweenStartAndStop);
            StartPin = true;
            await Task.Delay(StartPinHigh);
            StartPin = false;

            Status = GenStatus.Running;
        }
        public async Task Stop()
        {
            Status = GenStatus.Stopping;

            StopPin = true;
            await Task.Delay(StopPinHigh);
            StopPin = false;

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

        // http://www.magnum-dimensions.com/sites/default/files/MagAGS/ME-AGS-Onan-Models-HGJAD-HGJAE-HGJAF-Rev-12-02-08.pdf

        private readonly TimeSpan StopPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan StartPinHigh = TimeSpan.FromSeconds(10);
        private readonly TimeSpan DelayBetweenStartAndStop = TimeSpan.FromSeconds(4);
        private readonly TimeSpan DelayBetweenStartAttempts = TimeSpan.FromSeconds(4);

        private bool StopPin = false;
        private bool StartPin = false;

    }

    public enum GenStatus { Invalid = 0, Stopped, Starting, Running, Stopping };


}
