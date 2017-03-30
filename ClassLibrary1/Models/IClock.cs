using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHello.Portable.Models
{
    /// <summary>
    /// Provides a source of the current time
    /// </summary>
    public interface IClock
    {
        DateTime Now { get; }

        Task Delay(TimeSpan t);
    }
}
