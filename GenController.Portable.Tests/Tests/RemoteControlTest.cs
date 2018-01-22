using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenController.Portable.Tests.Mocks;
using GenController.Portable.Models;
using Common;
using System.Threading.Tasks;

namespace IotHello.Portable.Tests.Tests
{
    [TestClass]
    public class RemoteControlTest
    {
        private MockController Controller;
        private MockRemoteControlHWI RemoteControlHWI;
        private RemoteControlLogic RC;

        [TestInitialize]
        public void SetUp()
        {
            Service.Set<ISettings>(new MockSettings());
            Controller = new MockController();
            GenController.Portable.Models.Controller.Current = Controller;
            RemoteControlHWI = new MockRemoteControlHWI();
            GenController.Portable.Models.RemoteControlHWI.Current = RemoteControlHWI;
            RC = new RemoteControlLogic();
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
    }
}
