using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Uwp.Platform
{
    public class Clock : Portable.Models.IClock
    {
        public DateTime Now => DateTime.Now;

        public Task Delay(TimeSpan t) => Task.Delay(t);
    }
}
