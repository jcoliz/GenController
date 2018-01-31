using System;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Defines a platform-dependent service to log events and errors.
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
