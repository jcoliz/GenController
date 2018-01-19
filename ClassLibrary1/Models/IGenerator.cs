using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Defines the hardware interface to the generator
    /// </summary>
    public interface IGenerator: IVoltage
    {
        /// <summary>
        /// Status of the "Start" output line, true = high
        /// </summary>
        bool StartOutput { get;  set; }

        /// <summary>
        /// Status of the "Stop" output line, true = high
        /// </summary>
        bool StopOutput { get;  set; }

        /// <summary>
        /// Status of the "Run" input line, true = high
        /// </summary>
        bool RunInput { get; }
        
        /// <summary>
        /// Status of the "Panel Light" input line, true = high
        /// </summary>
        bool PanelLightInput { get; }

        /// <summary>
        /// Status of the "Comms" light on the control board
        /// </summary>
        bool CommsLight { set; }

        /// <summary>
        /// Status of the "Warn" light on the control board
        /// </summary>
        bool WarningLight { set; }
    }
}
