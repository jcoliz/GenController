using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commonality;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Specialized version of the file system logger, which outputs
    /// the current voltage to each logged line
    /// </summary>
    public class FileSystemLoggerWithVoltage: FileSystemLogger
    {
        public FileSystemLoggerWithVoltage(IClock clock, string homedir = null): base(clock, homedir)
        {
        }

        protected override string FormattedLine(string originalline) => Time.ToString("u") + " " + VoltageStr + originalline;

        private string VoltageStr => Voltage?.Voltage.ToString("0.0") + "V " ?? string.Empty;

        private IVoltage Voltage => Service.TryGet<IVoltage>();
    }
}
