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
            }
        }

        protected HardwareGenerator()
        {
        }

        public static async Task<HardwareGenerator> Open()
        {
            var result = new HardwareGenerator();
            result.Hat = await Pimoroni.MsIot.AutomationHat.Open();

            // These lights are SO bright! Tone them down a bit
            result.Hat.Light.Power.Value = 0.2;
            result.Hat.Relay[0].NO.Light.Brightness = 0.2;
            result.Hat.Relay[1].NO.Light.Brightness = 0.2;
            result.Hat.Input[1].Light.Brightness = 0.2;
            result.Hat.Input[2].Light.Brightness = 0.2;
            return result;
        }

        public void Dispose()
        {
            ((IDisposable)Hat).Dispose();
        }

        private Pimoroni.MsIot.AutomationHat Hat;

    }
}
