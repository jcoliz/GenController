using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotHello.Portable.Models;

namespace IotHello.Portable.Tests
{
    /// <summary>
    /// Mimic the controller interface for use in testing
    /// </summary>
    public class TestController : Models.IController
    {
        public string FullStatus
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GenStatus Status { get; set; } = GenStatus.Invalid;

        public async Task Start()
        {
            Status = GenStatus.Running;
        }

        public async Task Stop()
        {
            Status = GenStatus.Stopped;
        }
    }
}
