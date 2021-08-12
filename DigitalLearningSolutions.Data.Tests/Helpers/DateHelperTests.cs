namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DateHelperTests
    {
        [Test]
        [TestCase(ReportInterval.Days)]
        [TestCase(ReportInterval.Weeks)]
        [TestCase(ReportInterval.Months)]
        [TestCase(ReportInterval.Quarters)]
        [TestCase(ReportInterval.Years)]
        public void GetPeriodsBetweenDates_returns_single_entry_for_dates_in_same_period(ReportInterval interval)
        {
            // when
            var startDate = DateTime.Parse("2014-01-01 00:00:00.000");
            var endDate = DateTime.Parse("2014-01-01 23:59:59.999");
            var result = DateHelper.GetPeriodsBetweenDates(startDate, endDate, interval).ToList();

            // then
            result.Count.Should().Be(1);
        }

        [Test]
        [TestCase(ReportInterval.Days, "2014-01-03 23:59:59.999", "2014-01-01", "2014-01-03")]
        [TestCase(ReportInterval.Weeks, "2014-01-16 23:59:59.999", "2013-12-29", "2014-01-12")]
        [TestCase(ReportInterval.Months, "2014-03-31 23:59:59.999", "2014-01-01", "2014-03-01")]
        [TestCase(ReportInterval.Quarters, "2014-9-30 23:59:59.999", "2014-01-01", "2014-07-01")]
        [TestCase(ReportInterval.Years, "2016-12-31 23:59:59.999", "2014-01-01", "2016-01-01")]
        public void GetPeriodsBetweenDates_returns_list_inclusive_of_endpoints(
            ReportInterval interval,
            string endpointString,
            string expectedStartString,
            string expectedEndString
        )
        {
            // when
            var startDate = DateTime.Parse("2014-01-01 00:00:00.000");
            var endDate = DateTime.Parse(endpointString);
            var result = DateHelper.GetPeriodsBetweenDates(startDate, endDate, interval).ToList();

            // then
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                result.First().Date.Should().Be(DateTime.Parse(expectedStartString));
                result.Last().Date.Should().Be(DateTime.Parse(expectedEndString));
            }
        }
    }
}
