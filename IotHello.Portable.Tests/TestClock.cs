using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.Tests
{
    public class TestClock: Models.IClock
    {
        public DateTime Now { get; set; }
    }
}
