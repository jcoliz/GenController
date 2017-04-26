using ManiaLabs.Models;
using System;
using System.Threading.Tasks;

namespace Common
{
    public class Clock : IClock
    {
        public DateTime Now
        {
            get
            {
                if (!Offset.HasValue)
                {
                    Offset = TimeSpan.FromTicks(long.Parse( Setting.GetKeyValueWithDefault("Clock.Offset", "0")));
                }
                return DateTime.Now + Offset.Value;
            }
            set
            {
                Offset = value - DateTime.Now;
                Setting.SetKey("Clock.Offset", Offset.Value.Ticks.ToString());
            }
        }

        private TimeSpan? Offset;

        public Task Delay(TimeSpan t) => Task.Delay(t);
    }
}
