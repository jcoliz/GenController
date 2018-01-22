using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Remote control Hardware Interface
    /// </summary>
    /// <remarks>
    /// This listens to the physical remote control receiver hardware.
    /// The way the hardware is designed, there are four input lines, and one of them
    /// is always 'on' at any time, where the other three are off.
    /// </remarks>
    public class RemoteControlHWI: IRemoteControlHWI
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

        /// <summary>
        /// The current singleton instance of the HWI
        /// </summary>
        /// <remarks>
        /// This can be overridden with a Mock controller, to help with testing.
        /// </remarks>
        public static IRemoteControlHWI Current
        {
            get
            {
                if (null == _Current)
                    _Current = new RemoteControlHWI();
                return _Current;
            }

            set
            {
                // Setting the controller is allowed for platform injection during tests
                _Current = value;
            }
        }
        static IRemoteControlHWI _Current = null;

    }

    public interface IRemoteControlHWI
    {
        /// <summary>
        /// Whether the selected
        /// </summary>
        /// <remarks>
        /// There are four lines. Any or all of them can be pressed
        /// </remarks>
        bool IsPressed(int line);

        /// <summary>
        /// Raised when the a line changes state
        /// </summary>
        event EventHandler<int> LineChanged;
    }
}
