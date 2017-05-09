using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    /// <summary>
    /// Use this if you don't have generator hardware hooked up
    /// </summary>
    public class MockGenerator : IGenerator
    {
        public bool PanelLightInput { get; set; }

        public bool RunInput { get; set; }

        public bool StartOutput
        {
            get
            {
                return _StartOutput;
            }
            set
            {
                _StartOutput = value;
                if (value)
                    RunInput = true;
            }
        }
        private bool _StartOutput;

        public bool StopOutput
        {
            get
            {
                return _StopOutput;
            }
            set
            {
                _StopOutput = value;
                if (_StopOutput)
                {
                    RunInput = false;
                    PanelLightInput = true;
                }
                else
                    PanelLightInput = false;
            }
        }

        public bool CommsLight { get; set; } = false;

        public bool WarningLight { get; set; } = true;

        public double Voltage { get; set; } = 19.0;

        private bool _StopOutput;
    }
}
