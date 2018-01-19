using System;
using System.Threading.Tasks;
using GenController.Portable.Models;

namespace GenController.Portable.Tests.Mocks
{
    /// <summary>
    /// Mimic the controller interface for use in testing
    /// </summary>
    public class MockController : IController
    {
        public string FullStatus
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GenStatus Status { get; set; } = GenStatus.Invalid;

        public bool RunSignal { get; set; } = false;

        public double Voltage => throw new NotImplementedException();

        public async Task Start()
        {
            Status = GenStatus.Confirming;
        }

        public async Task Stop()
        {
            Status = GenStatus.Stopped;
        }

        public void Confirm()
        {
            if (Status == GenStatus.Confirming && RunSignal)
                Status = GenStatus.Running;
        }
    }
}
