namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
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
            );

            // then
            result.Count().Should().Be(expectedCount);
        }

        [Test]
        public void GetEvaluationSummaryData_fetches_data_correctly()
        {
            // Given
            var expectedResult = new EvaluationSummaryData
            {
                Q1No = 102,
                Q1Yes = 107,
                Q1NoResponse = 7,
                Q2No = 107,
                Q2Yes = 102,
                Q2NoResponse = 7,
                Q3No = 95,
                Q3Yes = 112,
                Q3NoResponse = 9,
                Q4Hrs0 = 95,
                Q4HrsLt1 = 54,
                Q4Hrs1To2 = 30,
                Q4Hrs2To4 = 16,
                Q4Hrs4To6 = 7,
                Q4HrsGt6 = 3,
                Q4NoResponse = 11,
                Q5No = 64,
                Q5Yes = 136,
                Q5NoResponse = 16,
                Q6NotApplicable = 23,
                Q6No = 64,
                Q6YesIndirectly = 86,
                Q6YesDirectly = 34,
                Q6NoResponse = 9,
                Q7No = 51,
                Q7Yes = 157,
                Q7NoResponse = 8
            };

            // When
            var result = service.GetEvaluationSummaryData(
                121,
                new DateTime(2010, 01, 01),
                new DateTime(2020, 01, 01),
                1,
                3,
                10059
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
