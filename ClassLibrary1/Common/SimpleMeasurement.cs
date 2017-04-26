using ManiaLabs.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// This is a simple implementation of the measurement interface which just
    /// logs to the local file system
    /// </summary>

    public class Logger: IMeasurement
    {
        private static string HomeDirectory = string.Empty;
        public Logger(string homedir = null)
        {
            if (!string.IsNullOrEmpty(homedir))
                HomeDirectory = homedir;
        }
        public void Error(string key, Exception ex)
        {
            var list = new List<string>();

            list.Add($"Error: {key}/{ex.GetType().ToString()}");
            if (ex.StackTrace?.Length > 0)
                list.Add($", Stack = {ex.StackTrace}");
            if (ex.Source?.Length > 0)
                list.Add($", Source = {ex.Source}");
            Exception e = ex;
            while (e != null)
            {
                list.Add($", Message = {e.GetType().ToString()} {e.Message}");
                e = e.InnerException;
            }
            var ignore = Log(list);
        }

        /// <summary>
        /// Async version for when you need to wait for it to get done
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>

        public void LogEvent(string message, params string[] parameters)
        {
            var list = new List<string>(parameters.Select(x => $", {x}"));
            list.Insert(0, $"Event: {message}");
            var ignore = Log(list);
        }

        public async Task LogEventAsync(string message, params string[] parameters)
        {
            var list = new List<string>(parameters.Select(x=>$", {x}"));
            list.Insert(0, $"Event: {message}");
            await Log(list);
        }

        public void LogInfo(string message)
        {
            var ignore = Log(new[] { $"FYI: {message}" });
        }

        public void SetLocation(IGeoPoint point, float accuracy)
        {
            var ignore = Log(new[] { $"Location: {point?.Latitude ?? '-'},{point?.Longitude ?? '-'}" });
        }

        public void StartSession()
        {
            var ignore = Log(new[] { "Started" });
        }

        /// <summary>
        /// Retrieve listing of all the log files
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetLogs() //=> await Filesystem.Directory("Logs");
        {
            var files = Directory.GetFiles(HomeDirectory + "Logs").Select(x => Path.GetFileName(x));
            return files;
        }

        /// <summary>
        /// Get a single log for reading
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static async Task<Stream> OpenLogForRead(DateTime dt)
        {
            var path = HomeDirectory + "Logs/" + dt.ToBinary().ToString("x") + ".txt";
            return File.OpenRead(path);
        }

        /// <summary>
        /// Filename of the session log file
        /// </summary>
        private string SessionFilename;

        SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        private async Task Log(IEnumerable<string> lines)
        {
            await Semaphore.WaitAsync();

            if (SessionFilename == null)
            {
                SessionFilename = "Logs/" + Time.ToBinary().ToString("x") + ".txt";

                try
                {
                    var path = HomeDirectory + SessionFilename;
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.CreateDirectory(dir);

                    using (var stream = File.Create(path))
                    {
                        var sw = new StreamWriter(stream);
                        await sw.WriteLineAsync(Time.ToString("u") + " Created");
                        await sw.FlushAsync();
                    }

                }
                catch (Exception)
                {

                }

            }

            try
            {
                var path = HomeDirectory + SessionFilename;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                using (var stream = new FileStream(path, FileMode.Append))
                {
                    var sw = new StreamWriter(stream);
                    foreach (var line in lines)
                        await sw.WriteLineAsync(Time.ToString("u") + " " + line);
                    await sw.FlushAsync();
                }

            }
            catch (Exception)
            {
                // All we can do is swallow exceptions here. We are deep INSIDE our exception
                // logging mechanism!
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// If there IS a platform clock use that for time, else just pick up regular
        /// system time.
        /// </summary>
        private DateTime Time => Platform.TryGet<IClock>()?.Now ?? DateTime.Now;
    }

    /// <summary>
    /// Wrap exceptions with this if we need to communicate to some other layer
    /// of code that the exception still needs to be sent to instrumentation.
    /// </summary>
    public class UnloggedException : Exception
    {
        public UnloggedException(string code, Exception inner): base(code,inner)
        {
        }
    }

}
