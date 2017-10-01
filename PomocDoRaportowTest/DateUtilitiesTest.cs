using System;
using NUnit.Framework;
using PomocDoRaprtow;

namespace PomocDoRaportowTest
{
    [TestFixture]
    public class DateUtilitiesTest
    {
        private DateTime Date(String date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        private void AssertResult(DateUtilities.ShiftInfo expected, String dateString)
        {
            DateUtilities.ShiftInfo shiftInfo = DateUtilities.DateToShiftInfo(Date(dateString));
            Assert.AreEqual(expected.DayOfTheMonth, shiftInfo.DayOfTheMonth);
            Assert.AreEqual(expected.Month, shiftInfo.Month);
            Assert.AreEqual(expected.ShiftNo, shiftInfo.ShiftNo);
        }

        [Test]
        public void Test1()
        {
            AssertResult(new DateUtilities.ShiftInfo(2, 10, 1), "2017-10-01 17:30");
        }

        [Test]
        public void Test2()
        {
            AssertResult(new DateUtilities.ShiftInfo(1, 10, 2), "2017-10-02 06:30");
        }

        [Test]
        public void Test3()
        {
            AssertResult(new DateUtilities.ShiftInfo(3, 10, 1), "2017-10-01 05:30");
        }

        [Test]
        public void Test4()
        {
            AssertResult(new DateUtilities.ShiftInfo(3, 1, 1), "2017-12-31 23:30");
        }
    }
}