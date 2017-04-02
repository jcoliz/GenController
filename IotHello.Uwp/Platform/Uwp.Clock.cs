using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Platform
{
    public class Clock : Portable.Models.IClock
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now + Offset;
            }
            set
            {
                Offset = value - DateTime.Now;
            }
        }

        // TODO: Save this to settings
        private TimeSpan Offset = TimeSpan.Zero;

        public Task Delay(TimeSpan t) => Task.Delay(t);
    }
}
