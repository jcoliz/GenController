using GenController.Portable.Models;
using IotFrosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Uwp.Platform
{
    /// <summary>
    /// Hardware Interface to the physical remote
    /// </summary>
    /// <remarks>
    /// This listens to the physical remote control receiver hardware.
    /// The way the hardware is designed, there are four input lines, and one of them
    /// is always 'on' at any time, where the other three are off.
    /// </remarks>
    public class HardwareRemote: IRemote, IDisposable
    {
        List<IPin> Pins = new List<IPin>() { new InputPin(1), new InputPin(2) };
        
        public HardwareRemote()
        {
            foreach (var pin in Pins)
                pin.Updated += Pin_Updated;
        }

        private void Pin_Updated(IInput sender, EventArgs args)
        {
            var index = Pins.IndexOf(sender as IPin);

            LineChanged?.Invoke(this, index + 1);
        }

        /// <summary>
        /// Whether the selected
        /// </summary>
        /// <remarks>
        /// There are four lines. Any or all of them can be pressed
        /// </remarks>
        public bool IsPressed(int line)
        {
            return Pins[line - 1].State;
        }

        /// <summary>
        /// Raised when the a line changes state
        /// </summary>
        public event EventHandler<int> LineChanged;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    foreach( var pin in Pins )
                    {
                        pin.Dispose();
                    }
                    Pins.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HardwareRemote() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
