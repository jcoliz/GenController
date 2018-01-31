using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Provides a way to control the generator remotely using an RF remote
    /// </summary>
    /// <remarks>
    /// This is built with a latching RF receiver with two lines. A "start" button
    /// line and a "stop" button line. We act when there are changes in those lines.
    /// 
    /// The hardware interface to the remote is found in an IRemote. 
    /// This should already be set in the service locator becore calling
    /// AttachToHardware()
    /// 
    /// However, if there is no IRemote (perhaps because the app is running
    /// natively with no actual GPIO), this class will do nothing.
    /// </remarks>
    public class RemoteControlLogic
    {
        /// <summary>
        /// Attach to the underlying hardware
        /// </summary>
        /// <remarks>
        /// Make sure the IRemote service is set first, if there is ever going to be one.
        /// </remarks>
        public void AttachToHardware()
        {
            // It's OK if there is no hardware remote
            if (null != Remote)
                Remote.LineChanged += Remote_LineChanged;
        }

        /// <summary>
        /// The current singleton
        /// </summary>
        public static RemoteControlLogic Current
        {
            get
            {
                if (null == _Current)
                    _Current = new RemoteControlLogic();
                return _Current;
            }
        }
        static RemoteControlLogic _Current = null;

        private void Remote_LineChanged(object sender, int line)
        {
            if (line == 1 && Remote.IsPressed(1))
                Controller.Current.Start();
            else if (line == 2 && Remote.IsPressed(2))
                Controller.Current.Stop();
        }

        private IRemote Remote => Service.TryGet<IRemote>();
    }
}
