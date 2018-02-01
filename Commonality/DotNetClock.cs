using System;
using System.Threading.Tasks;

namespace Commonality
{
    /// <summary>
    /// Provides a standard dotnet 'DateTime.Now' in the IClock interface
    /// </summary>
    /// <remarks>
    /// This is helpful both for testing, and for a system where we use an external clock
    /// </remarks>
    public class Clock : IClock
    {
        public DateTime Now
        {
            get
            {
                if (!Offset.HasValue)
                {
                    Offset = TimeSpan.FromTicks(long.Parse( Settings.GetKey("Clock.Offset") ?? "0"));
                }
                return DateTime.Now + Offset.Value;
            }
            set
            {
                Offset = value - DateTime.Now;
                Settings.SetKey("Clock.Offset", Offset.Value.Ticks.ToString());
            }
        }

        private ISettings Settings => Service.Get<ISettings>();

        private TimeSpan? Offset;

        public Task Delay(TimeSpan t) => Task.Delay(t);
    }
}
