using Common;
using System;
using System.Threading.Tasks;

namespace IotHello.Portable.Tests
{
    public class TestClock: IClock
    {
        public DateTime Now { get; set; }

        public async Task Delay(TimeSpan t)
        {
            Now += t;
        }
    }
}
