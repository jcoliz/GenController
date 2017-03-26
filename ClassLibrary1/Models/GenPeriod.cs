using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    /// <summary>
    /// Describes a period of time during the day when the generator should be on
    /// </summary>
    public class GenPeriod
    {
        /// <summary>
        /// Offset from midnight when the generator should start
        /// </summary>
        public TimeSpan StartAt { get; set; }

        /// <summary>
        /// Offset from midnight when the generator should stop
        /// </summary>
        public TimeSpan StopAt { get; set; }

        public GenPeriod(TimeSpan start, TimeSpan stop)
        {
            StartAt = start;
            StopAt = stop;
        }

        public string Label => StartAt.ToString("hh\\:mm") + Environment.NewLine + StopAt.ToString("hh\\:mm");
    }
}
