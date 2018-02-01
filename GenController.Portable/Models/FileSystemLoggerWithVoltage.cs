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
        public FileSystemLoggerWithVoltage(string homedir = null): base(homedir)
        {
        }

        protected override string FormattedLine(string originalline) => Time.ToString("u") + " " + Voltage + originalline;

        private string Voltage => Service.TryGet<IVoltage>()?.Voltage.ToString("0.0") + "V " ?? string.Empty;
    }
}
