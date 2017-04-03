using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
