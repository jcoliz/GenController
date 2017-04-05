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
                return Hat.Relay[1].State;
            }
            set
            {
                Hat.Relay[1].State = value;
            }
        }

        public bool StopOutput
        {
            get
            {
                return Hat.Relay[2].State;
            }
            set
            {
                Hat.Relay[2].State = value;
            }
        }

        protected HardwareGenerator()
        {
        }

        public static async Task<HardwareGenerator> Open()
        {
            var result = new HardwareGenerator();
            result.Hat = await Pimoroni.MsIot.AutomationHat.Open();
            return result;
        }

        public void Dispose()
        {
            ((IDisposable)Hat).Dispose();
        }

        private Pimoroni.MsIot.AutomationHat Hat;

    }
}
