using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Defines the hardware interface to a remote
    /// </summary>
    public interface IRemote
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
