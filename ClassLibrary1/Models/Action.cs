using Common.Portable.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IotHello.Portable.Models
{
    public class Action
    {
        public DelegateCommand Command { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
    }
}
