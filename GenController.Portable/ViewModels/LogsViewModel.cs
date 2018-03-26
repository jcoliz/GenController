using Commonality;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GenController.Portable.ViewModels
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
                List<DateTime> result = FileSystemLogger.GetLogs().ToList();

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
                var result = await FileSystemLogger.ReadContents(session);
                Log.Clear();
                Log.AddRange(result);
                SelectedItem = 0;
                base.SetProperty(nameof(SelectedItem));
            }
            catch (Exception ex)
            {
                SetError("LV2", ex);
            }
        }

        public int SelectedItem { get; set; } = -1;

        public ICommand PageUpCommand => new DelegateCommand(_ => 
        {
            try
            {
                int desired = SelectedItem - 15;
                if (desired < 0)
                    desired = 0;
                if (desired != SelectedItem)
                {
                    SelectedItem = desired;
                    base.SetProperty(nameof(SelectedItem));
                }
            }
            catch (Exception ex)
            {
                SetError("LV3", ex);
            }
        });
        public ICommand PageDownCommand => new DelegateCommand(_ =>
        {
            try
            {
                int desired = SelectedItem + 15;
                if (desired >= Log.Count)
                    desired = Log.Count - 1;
                if (desired != SelectedItem)
                {
                    SelectedItem = desired;
                    base.SetProperty(nameof(SelectedItem));
                }
            }
            catch (Exception ex)
            {
                SetError("LV4", ex);
            }
        });
    }
}
