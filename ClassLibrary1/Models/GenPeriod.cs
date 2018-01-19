using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenController.Portable.Models
{
    /// <summary>
    /// Describes a period of time during the day when the generator should be on
    /// </summary>
    public class GenPeriod: IComparable<GenPeriod>
    {
        /// <summary>
        /// Offset from midnight when the generator should start
        /// </summary>
        public TimeSpan StartAt { get; set; }

        /// <summary>
        /// Offset from midnight when the generator should stop
        /// </summary>
        public TimeSpan StopAt { get; set; }

        /// <summary>
        /// Voltage we should be under for this period to start, or 0.0 
        /// means "start no matter the voltage"
        /// </summary>
        public double Voltage { get; set; } = 14.0;

        public GenPeriod(TimeSpan start, TimeSpan stop, double voltage)
        {
            StartAt = start;
            StopAt = stop;
            Voltage = voltage;
        }
        public GenPeriod(string serializekey)
        {
            var split = serializekey.Split(' ');
            StartAt = TimeSpan.Parse(split[0]);
            StopAt = TimeSpan.Parse(split[1]);
            if (split.Length >= 3)
                Voltage = double.Parse(split[2]);
        }

        public string Label => StartAt.ToString("hh\\:mm") + Environment.NewLine + StopAt.ToString("hh\\:mm");

        public string SerializeKey => StartAt.ToString("hh\\:mm") + " " + StopAt.ToString("hh\\:mm") + " " + Voltage.ToString("0.0");

        public int CompareTo(GenPeriod other)
        {
            var result = StartAt.CompareTo(other.StartAt);
            if (result == 0)
                result = StopAt.CompareTo(other.StopAt);

            return result;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GenPeriod))
                return false;
            return 0 == CompareTo(obj as GenPeriod);
        }

        public override int GetHashCode() => StartAt.GetHashCode() ^ StopAt.GetHashCode();

        static public string Serialize(GenPeriod x) => x.SerializeKey;

        static public GenPeriod Deserialize(string x) => new GenPeriod(x);
    }
}
