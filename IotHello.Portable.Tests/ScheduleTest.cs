﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

// Not needed to await here in the tests, because the TestController executes generator control commands
// immediately.
#pragma warning disable 1998
#pragma warning disable 4014

namespace IotHello.Portable.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        private TestClock Clock = new TestClock();
        private TestController Controller;

        [TestInitialize]
        public void SetUp()
        {
            new Models.Schedule();
            Models.Schedule.Current.Clock = Clock;
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(7), TimeSpan.FromHours(9)));
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14)));
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(17), TimeSpan.FromHours(19)));
            Models.Controller.Current = Controller = new TestController() { Status = Models.GenStatus.Invalid };
        }

        [TestMethod]
        public async Task StartingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StoppingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 08, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 09, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }
        [TestMethod]
        public async Task LastStartingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 16, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 17, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task LastStoppingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 18, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 19, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Stopped, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task Midnight()
        {
            Clock.Now = new DateTime(2017, 3, 1, 11, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 2, 00, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Invalid, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task DuringPeriodNoChange()
        {
            Clock.Now = new DateTime(2017, 3, 1, 07, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 2, 08, 00, 01);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Invalid, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StartingEdgeFails()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            Models.Schedule.Current.Tick();
            Controller.Status = Models.GenStatus.FailedToStart;
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 02);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 01, 03);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.FailedToStart, Models.Controller.Current.Status);
        }

        [TestMethod]
        public async Task StartingEdgeFailsAndRetries()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            Models.Schedule.Current.Tick();
            Controller.Status = Models.GenStatus.FailedToStart;
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 02);
            Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 02, 03);
            Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }

    }
}

#pragma warning restore