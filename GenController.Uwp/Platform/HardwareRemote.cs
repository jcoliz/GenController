using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Hardware Interface to the physical remote
    /// </summary>
    /// <remarks>
    /// This listens to the physical remote control receiver hardware.
    /// The way the hardware is designed, there are four input lines, and one of them
    /// is always 'on' at any time, where the other three are off.
    /// </remarks>
    public class HardwareRemote: IRemote
    {
        /// <summary>
        /// Whether the selected
        /// </summary>
        /// <remarks>
        /// There are four lines. Any or all of them can be pressed
        /// </remarks>
        public bool IsPressed(int line)
        {
            return false;
        }

        /// <summary>
        /// Raised when the a line changes state
        /// </summary>
        public event EventHandler<int> LineChanged;
    }

}
