using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Platform
{
    /// <summary>
    /// Controls the actual generator hooked up to real hardware
    /// </summary>
    public class HardwareGenerator : Portable.Models.IGenerator, IDisposable
    {
        /// <summary>
        /// Reads the 'F' line: Whether the generator is sufficiently primed to start
        /// </summary>
        public bool PanelLightInput
        {
            get
            {
                return Hat.Input[2].State;
            }
        }

        /// <summary>
        /// Reads the 'E' line: Whether the generator is running
        /// </summary>
        public bool RunInput
        {
            get
            {
                return Hat.Input[1].State;
            }
        }

        /// <summary>
        /// Connects 'C' to 'A' to start the generator
        /// </summary>
        public bool StartOutput
        {
            get
            {
                return Hat.Relay[1].State;
            }
            set
            {
                Hat.Relay[1].State = value;

                // Also set the output. Testing to see if I can use outputs instead of relays.
                Hat.Output[1].State = value;
            }
        }

        /// <summary>
        /// Connects 'B' to 'A' to stop the generator
        /// </summary>
        public bool StopOutput
        {
            get
            {
                return Hat.Relay[0].State;
            }
            set
            {                
                Hat.Relay[0].State = value;

                // Also set the output. Testing to see if I can use outputs instead of relays.
                Hat.Output[0].State = value;
            }
        }

        /// <summary>
        /// Status of the "Comms" light on the control board
        /// </summary>
        public bool CommsLight
        {
            set
            {
                Hat.Light.Comms.State = value;
            }
        }

        /// <summary>
        /// Status of the "Warn" light on the control board
        /// </summary>
        public bool WarningLight
        {
            set
            {
                Hat.Light.Warn.State = value;
            }
        }

        /// <summary>
        /// Current voltage sensed in the system
        /// </summary>
        public double Voltage => Hat.Analog[0].Voltage;

        protected HardwareGenerator()
        {
        }

        public static async Task<HardwareGenerator> Open()
        {
            var result = new HardwareGenerator();
            result.Hat = await IotFrosting.Pimoroni.AutomationHat.Open();

            // These lights are SO bright! Tone them down a bit
            result.Hat.Light.Power.Value = 0.2;
            result.Hat.Light.Warn.Value = 0.2;
            result.Hat.Light.Comms.Brightness = 0.2;
            result.Hat.Light.Power.Value = 0.2;
            result.Hat.Relay[0].NO.Light.Brightness = 0.2;
            result.Hat.Relay[1].NO.Light.Brightness = 0.2;
            result.Hat.Input[1].Light.Brightness = 0.2;
            result.Hat.Input[2].Light.Brightness = 0.2;
            result.Hat.Output[0].Light.Brightness = 0.2;
            result.Hat.Output[1].Light.Brightness = 0.2;
            return result;
        }

        public void Dispose()
        {
            ((IDisposable)Hat).Dispose();
        }

        private IotFrosting.Pimoroni.AutomationHat Hat;

    }
}
