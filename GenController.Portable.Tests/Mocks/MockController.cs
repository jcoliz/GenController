using System;
using System.ComponentModel;
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

        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore

#pragma warning disable 1998
        public async Task Start()
        {
            Status = GenStatus.Confirming;
        }

        public async Task Stop()
        {
            Status = GenStatus.Stopped;
        }
#pragma warning restore 1998

        public void Confirm()
        {
            if (Status == GenStatus.Confirming && RunSignal)
                Status = GenStatus.Running;
        }
    }
}
