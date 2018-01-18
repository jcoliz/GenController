using System;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Provides a source of the current time
    /// </summary>
    public interface IClock
    {
        DateTime Now { get; set; }

        Task Delay(TimeSpan t);
    }
}
