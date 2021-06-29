namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    class ActivityDataServiceTests
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
            var start = DateTime.Parse("2014-01-01 00:00:00.000");
            var end = DateTime.Parse("2014-04-30 23:59:59.999");
            var result = service.GetActivityInRangeByMonth(101, start, end).ToList();

            // then
            result.Count().Should().Be(4);

            var first = result.First();
            first.Year.Should().Be(2014);
            first.Month.Should().Be(1);
            first.Completions.Should().Be(1);
            first.Evaluations.Should().Be(0);
            first.Registrations.Should().Be(12);

            var last = result.Last();
            last.Year.Should().Be(2014);
            last.Month.Should().Be(4);
            last.Completions.Should().Be(0);
            last.Evaluations.Should().Be(1);
            last.Registrations.Should().Be(7);
        }
    }
}
