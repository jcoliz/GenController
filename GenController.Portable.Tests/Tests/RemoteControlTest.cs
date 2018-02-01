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
            Service.Set<ISettings>(new MockSettings());
            Controller = new MockController();
            GenController.Portable.Models.Controller.Current = Controller;
            RemoteControlHWI = new MockRemoteControlHWI();
            Service.Set<IRemote>(RemoteControlHWI);
            RC = new RemoteControlLogic();
            RC.AttachToHardware();
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
