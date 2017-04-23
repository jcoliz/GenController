using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotFrosting;

namespace IotHello.Uwp.Platform
{
    class HardwareClock : ManiaLabs.Portable.Base.IClock
    {
        private DS3231 ds;

        public DateTime Now
        {
            get
            {
                return ds?.Now ?? DateTime.MinValue;
            }

            set
            {
                if (ds != null)
                    ds.Now = value;
            }
        }

        public static async Task<HardwareClock> Open()
        {
            var result = new HardwareClock();
            result.ds = await DS3231.Open();

            return result;
        }
        public async Task Delay(TimeSpan t)
        {
            var start = ds.Now;
            while (ds.Now - start < t)
            {
                int ms = Math.Max(20, (int)((ds.Now - start).TotalMilliseconds * 0.9));
                await Task.Delay((int)ms);
            }
        }

        public void Tick()
        {
            ds.Tick();
        }
    }
}
