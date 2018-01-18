using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IotHello.Portable.Models;
using System.Collections.Generic;
using System.Linq;

namespace IotHello.Portable.Tests
{
    [TestClass]
    public class GenPeriodTest
    {
        [TestMethod]
        public void Deserialize()
        {
            var expected = new GenPeriod(TimeSpan.FromHours(2.5), TimeSpan.FromHours(21.25),0.0);

            var serialize = expected.SerializeKey;

            var actual = new GenPeriod(serialize);

            Assert.AreEqual(expected.StartAt, actual.StartAt);
            Assert.AreEqual(expected.StopAt, actual.StopAt);
        }

        [TestMethod]
        public void Bulk()
        {
            var expected = new List<GenPeriod>();
            var current = TimeSpan.FromHours(5);
            var period = TimeSpan.FromMinutes(10);
            var ending = TimeSpan.FromHours(22);
            while (current < ending)
            {
                expected.Add(new Portable.Models.GenPeriod(current, current + period,0.0));
                current += period + period;
            }

            var storage = expected.Select(GenPeriod.Serialize);

            var actual = storage.Select(GenPeriod.Deserialize).ToList();

            Assert.AreEqual(expected.Count, actual.Count);
            int i = expected.Count;
            while (i-- > 0)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
