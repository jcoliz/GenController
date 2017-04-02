using ManiaLabs.Helpers;
using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IotHello.Portable.ViewModels
{
    /// <summary>
    /// This makes the logs from SimpleMeasurement instrumentation visible
    /// </summary>
    public class LogsViewModel: ViewModelBase
    {
        /// <summary>
        /// List of session logs available
        /// </summary>
        public RangeObservableCollection<DateTime> Sessions { get; } = new RangeObservableCollection<DateTime>();

        /// <summary>
        /// Details of currently-selected log
        /// </summary>
        public RangeObservableCollection<string> Log { get; } = new RangeObservableCollection<string>();

        /// <summary>
        /// Load the sessions up from disk
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            try
            {
                List<DateTime> result = new List<DateTime>();

                var logs = await SimpleMeasurement.GetLogs();
                foreach (var log in logs)
                {
                    var text = log.Split('.')[0];
                    long binary = Convert.ToInt64(text, 16);
                    DateTime dt = DateTime.FromBinary(binary);
                    result.Add(dt);
                }
                // Sort with most recent on top
                result.Sort((x,y)=>y.CompareTo(x));
                Sessions.Clear();
                Sessions.AddRange(result);
            }
            catch (Exception ex)
            {
                SetError("LV1", ex);
            }
        }

        /// <summary>
        /// Load the details from the indicated session log into view
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task SelectSession(DateTime session)
        {
            try
            {
                List<string> result = new List<string>();

                using (var stream = await SimpleMeasurement.OpenLogForRead(session))
                {
                    var reader = new StreamReader(stream);
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        result.Add(line);
                    }
                }

                Log.Clear();
                Log.AddRange(result);
            }
            catch (Exception ex)
            {
                SetError("LV2", ex);
            }
        }
    }
}
