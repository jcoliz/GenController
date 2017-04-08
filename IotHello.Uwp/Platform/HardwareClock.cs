using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Platform
{
    class HardwareClock : Pimoroni.MsIot.DS3231, ManiaLabs.Portable.Base.IClock
    {
        public async Task Delay(TimeSpan t)
        {
            var start = Now;
            while (Now - start < t)
            {
                int ms = Math.Max(20, (int)((Now - start).TotalMilliseconds * 0.9));
                await Task.Delay((int)ms);
            }
        }
    }
}
