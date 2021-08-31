namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
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
        public void GetFilteredActivity_gets_expected_activity()
        {
            // when
            var result = service.GetFilteredActivity(
                    101,
                    DateTime.Parse("2014-01-01 00:00:00.000"),
                    DateTime.Parse("2014-01-31 23:59:59.999"),
                    null,
                    null,
                    null
                )
                .OrderBy(log => log.LogDate)
                .ToList();

            // then
            using (new AssertionScope())
            {
                result.Count().Should().Be(13);

                var first = result.First();
                first.LogDate.Should().Be(DateTime.Parse("2014-01-08 11:04:35.753"));
                first.LogYear.Should().Be(2014);
                first.LogQuarter.Should().Be(1);
                first.LogMonth.Should().Be(1);
                first.Completed.Should().BeFalse();
                first.Evaluated.Should().BeFalse();
                first.Registered.Should().BeTrue();

                var last = result.Last();
                last.LogDate.Should().Be(DateTime.Parse("2014-01-31 09:43:28.840"));
                last.LogYear.Should().Be(2014);
                last.LogQuarter.Should().Be(1);
                last.LogMonth.Should().Be(1);
                last.Completed.Should().BeFalse();
                last.Evaluated.Should().BeFalse();
                last.Registered.Should().BeTrue();
            }
        }

        [Test]
        [TestCase(67, null, null, null)]
        [TestCase(2, 10, null, null)]
        [TestCase(42, null, 3, null)]
        [TestCase(3, null, null, 7832)]
        public void GetFilteredActivity_filters_data_correctly(
            int expectedCount,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId
        )
        {
            // when
            var result = service.GetFilteredActivity(
                    101,
                    DateTime.Parse("2014-01-01 00:00:00.000"),
                    DateTime.Parse("2014-03-31 23:59:59.999"),
                    jobGroupId,
                    courseCategoryId,
                    customisationId
                )
                .OrderBy(log => log.LogDate);

            // then
            result.Count().Should().Be(expectedCount);
        }
    }
}
