using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace IotHello.Portable.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        private TestClock Clock = new TestClock();

        [TestInitialize]
        public void SetUp()
        {
            new Models.Schedule();
            Models.Schedule.Current.Clock = Clock;
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(7), TimeSpan.FromHours(9)));
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14)));
            Models.Schedule.Current.Periods.Add(new Models.GenPeriod(TimeSpan.FromHours(17), TimeSpan.FromHours(19)));
            Models.Controller.Current = new TestController();
        }

        [TestMethod]
        public async Task StartingEdge()
        {
            Clock.Now = new DateTime(2017, 3, 1, 06, 59, 58);
            await Models.Schedule.Current.Tick();
            Clock.Now = new DateTime(2017, 3, 1, 07, 00, 01);
            await Models.Schedule.Current.Tick();

            Assert.AreEqual(Models.GenStatus.Running, Models.Controller.Current.Status);
        }
    }
}
