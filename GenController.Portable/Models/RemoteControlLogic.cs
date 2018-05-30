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
    /// </remarks>
    public class RemoteControlLogic
    {
        public RemoteControlLogic(IRemote remote, IController controller)
        {
            remote.LineChanged += (s,line) =>
            {
                if (line == 1 && remote.IsPressed(1))
                    controller.Start();
                else if (line == 2 && remote.IsPressed(2))
                    controller.Stop();
            };
        }
    }
}
