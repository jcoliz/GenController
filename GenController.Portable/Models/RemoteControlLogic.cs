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
    /// </remarks>
    public class RemoteControlLogic
    {
        public RemoteControlLogic()
        {
            Remote.LineChanged += Remote_LineChanged;
        }

        private void Remote_LineChanged(object sender, int line)
        {
            if (line == 1 && Remote.IsPressed(1))
                Controller.Current.Start();
        }

        private IRemote Remote => Service.Get<IRemote>();
    }
}
