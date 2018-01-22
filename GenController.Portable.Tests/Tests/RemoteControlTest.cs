using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenController.Portable.Tests.Mocks;
using GenController.Portable.Models;
using Common;

namespace IotHello.Portable.Tests.Tests
{
    [TestClass]
    public class RemoteControlTest
    {
        private MockController Controller;
        private RemoteControl RC;

        [TestInitialize]
        public void SetUp()
        {
            Service.Set<ISettings>(new MockSettings());
            Controller = new MockController();
            GenController.Portable.Models.Controller.Current = Controller;
            RC = new RemoteControl();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(RC);
        }
    }
}
