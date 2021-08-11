namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ActivityDataServiceTests
    {
        private IActivityDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new ActivityDataService(connection);
        }

        [Test]
        public void GetActivityForMonthsInYear_gets_activity_by_month_for_date_range()
        {
            // when
            // var start = DateTime.Parse("2014-01-01 00:00:00.000");
            // var end = DateTime.Parse("2014-04-30 23:59:59.999");
            var filterData = new ActivityFilterData
            {
                StartDate = DateTime.Parse("2014-01-01 00:00:00.000"),
                EndDate = DateTime.Parse("2014-01-31 23:59:59.999"),
                ReportInterval = ReportInterval.Months
            };
            var result = service.GetRawActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                result.Count.Should().Be(13);

                var first = result.First();
                first.LogDate.Should().Be(DateTime.Now);
                first.LogYear.Should().Be(1);
                first.LogQuarter.Should().Be(1);
                first.LogMonth.Should().Be(1);
                first.Completed.Should().Be(1);
                first.Evaluated.Should().Be(0);
                first.Registered.Should().Be(12);

                var last = result.Last();
                last.LogDate.Should().Be(DateTime.Now);
                last.LogYear.Should().Be(1);
                last.LogQuarter.Should().Be(1);
                last.LogMonth.Should().Be(1);
                last.Completed.Should().Be(1);
                last.Evaluated.Should().Be(0);
                last.Registered.Should().Be(12);
            }
        }
    }
}
