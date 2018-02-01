using System;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Defines a platform-dependent service to access the time
    /// </summary>
    public interface IClock
    {
        DateTime Now { get; set; }

        Task Delay(TimeSpan t);
    }
}
