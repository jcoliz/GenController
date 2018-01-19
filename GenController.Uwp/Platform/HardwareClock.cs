using System;
using System.Threading.Tasks;
using IotFrosting;
using Common;

namespace GenController.Uwp.Platform
{
    /// <summary>
    /// Wraps the DS3231 controller in an IClock interface so the rest of the system can use it
    /// </summary>
    class HardwareClock : IClock
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
