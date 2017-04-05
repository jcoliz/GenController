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
        public bool PrimedInput
        {
            get
            {
                return Hat.Input[1].State;
            }
        }

        public bool RunInput
        {
            get
            {
                return Hat.Input[2].State;
            }
        }

        public bool StartOutput
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

        public bool StopOutput
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

        protected HardwareGenerator()
        {
        }

        public static async Task<HardwareGenerator> Open()
        {
            var result = new HardwareGenerator();
            result.Hat = await Pimoroni.MsIot.AutomationHat.Open();

            // These lights are SO bright! Tone them down a bit
            result.Hat.Light.Power.Value = 0.05;
            result.Hat.Relay[0].NO.Light.Brightness = 0.05;
            result.Hat.Relay[1].NO.Light.Brightness = 0.05;
            result.Hat.Input[1].Light.Brightness = 0.05;
            result.Hat.Input[2].Light.Brightness = 0.05;
            return result;
        }

        public void Dispose()
        {
            ((IDisposable)Hat).Dispose();
        }

        private Pimoroni.MsIot.AutomationHat Hat;

    }
}
