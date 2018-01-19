using Common;
using System;
using System.Threading.Tasks;

namespace GenController.Portable.Tests.Mocks
{
    public class MockClock: IClock
    {
        public DateTime Now { get; set; }

#pragma warning disable 1998
        public async Task Delay(TimeSpan t)
        {
            Now += t;
        }
#pragma warning restore 1998
    }
}
