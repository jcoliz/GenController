using System;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Interface for app logging and analytics. Actual implementation is platform-dependent
    /// </summary>
    public interface ILogger
    {
        void StartSession();
        void Error(string key, Exception ex);
        Task LogEventAsync(string message, params string[] parameters);
        void LogEvent(string message, params string[] parameters);
        void LogInfo(string message);
    }
}
