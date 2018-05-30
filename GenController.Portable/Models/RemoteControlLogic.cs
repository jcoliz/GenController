using Commonality;

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
    ///
    /// Service Dependencies:
    ///     * IRemote
    ///     * IController
    /// </remarks>
    public class RemoteControlLogic
    {
        public RemoteControlLogic(IRemote remote, IController controller)
        {
            Remote = remote;
            Controller = controller;
        }
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

        private void Remote_LineChanged(object sender, int line)
        {
            if (line == 1 && Remote.IsPressed(1))
                Controller.Start();
            else if (line == 2 && Remote.IsPressed(2))
                Controller.Stop();
        }

        private IRemote Remote;

        private IController Controller;
    }
}
