using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using GenController.Portable.Tests.Mocks;
using Commonality;
using GenController.Portable.Models;

// Not needed to await here in the tests, because the TestController executes generator control commands
// immediately.
#pragma warning disable 1998
#pragma warning disable 4014

namespace GenController.Portable.Tests.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        private MockClock Clock { get; set; }
        private MockController Controller;
        private Schedule ScheduleCurrent {get;set;}

        [TestInitialize]
        public void SetUp()
        {
            ScheduleCurrent = new Schedule();
            Service.Set<IClock>(Clock = new MockClock());
            Service.Set<ISettings>(new MockSettings());
            ScheduleCurrent.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(7), TimeSpan.FromHours(9),0.0));
            ScheduleCurrent.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14),0.0));
            ScheduleCurrent.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(17), TimeSpan.FromHours(19),0.0));
            Models.Controller.Current = Controller = new MockController() { Status = Models.GenStatus.Invalid };
        }

        [TestMethod]
        public async Task StartingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 02);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StoppingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 08, 59, 58);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 1, 09, 00, 01);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 09, 00, 02);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }
        [TestMethod]
        public async Task LastStartingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 16, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 17, 00, 01);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 1, 17, 00, 02);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task LastStoppingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 18, 59, 56);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 1, 18, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 19, 00, 01);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task Midnight()
        {
            Clock.Now = new DateTime(2017, 3, 1, 11, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 2, 00, 00, 01);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task DuringPeriodNoChange()
        {
            Clock.Now = new DateTime(2017, 3, 1, 07, 59, 58);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 2, 08, 00, 01);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StartingEdgeFails()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 02);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 01, 03);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Confirming, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StartingEdgeFailsAndRetries()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 02);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 02, 03);
            ScheduleCurrent.Tick();
            Controller.RunSignal = true;
            Clock.Now = new DateTime(2017, 3, 1, 07, 02, 04);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task RespectsManualStart()
        {
            // Make sure the schedule doesn't override the user's manual start
            //
            // I realized this is a bug in my new start/stop logic!!

            // This is well within the period where the schedule says to be off
            Clock.Now = new DateTime(2017, 3, 1, 10, 00, 01);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 10, 00, 02);
            ScheduleCurrent.Tick();

            // Start the controller
            ScheduleCurrent.Override();
            await Controller.Start();
            Controller.RunSignal = true;

            Clock.Now = new DateTime(2017, 3, 1, 10, 01, 00);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 10, 01, 02);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task GetsPastManualStart()
        {
            // Make sure the schedule doesn't override the user's manual start
            //
            // I realized this is a bug in my new start/stop logic!!

            // This is well within the period where the schedule says to be off
            Clock.Now = new DateTime(2017, 3, 1, 10, 00, 01);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 10, 00, 02);
            ScheduleCurrent.Tick();

            // Start the controller
            ScheduleCurrent.Override();
            await Controller.Start();
            Controller.RunSignal = true;

            Clock.Now = new DateTime(2017, 3, 1, 10, 01, 00);
            ScheduleCurrent.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 10, 01, 02);
            ScheduleCurrent.Tick();

            // Now we are running
            // Let's make sure we stop OK!
            Clock.Now = new DateTime(2017, 3, 1, 14, 01, 02);
            ScheduleCurrent.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }

    }
}

#pragma warning restore