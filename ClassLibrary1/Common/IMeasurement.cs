using ManiaLabs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Interface for app analytics. Actual implementation is platform-dependent
    /// </summary>
    public interface IMeasurement
    {
        void StartSession();
        void Error(string key, Exception ex);
        Task LogEventAsync(string message, params string[] parameters);
        void LogEvent(string message, params string[] parameters);
        void LogInfo(string message);
        void SetLocation(IGeoPoint point, float accuracy);
    }
}
