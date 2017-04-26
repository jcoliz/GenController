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
