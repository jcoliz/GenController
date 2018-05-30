using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenController.Portable.Tests.Mocks;
using GenController.Portable.Models;
using Commonality;
using System.Threading.Tasks;

namespace IotHello.Portable.Tests.Tests
{
    [TestClass]
    public class RemoteControlTest
    {
        private RemoteControlLogic RC;
        private MockController Controller;
        private MockRemoteControlHWI RemoteControlHWI;

        [TestInitialize]
        public void SetUp()
        {
            Controller = new MockController();
            RemoteControlHWI = new MockRemoteControlHWI();
            RC = new RemoteControlLogic(RemoteControlHWI,Controller);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(RC);
        }

        [TestMethod]
        public async Task StartFromStopped()
        {
            await Controller.Stop();
            RemoteControlHWI.SetPressed(1, true);

            Assert.AreEqual(GenStatus.Confirming, Controller.Status);
        }
        [TestMethod]
        public async Task StopFromRunning()
        {
            await Controller.Start();
            Controller.Confirm();
            RemoteControlHWI.SetPressed(2, true);

            Assert.AreEqual(GenStatus.Stopped, Controller.Status);
        }
    }
}
