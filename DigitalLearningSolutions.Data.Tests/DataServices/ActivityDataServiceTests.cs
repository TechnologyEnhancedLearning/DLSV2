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
                Q1No = 664,
                Q1Yes = 627,
                Q1NoResponse = 35,
                Q2No = 739,
                Q2Yes = 546,
                Q2NoResponse = 41,
                Q3No = 723,
                Q3Yes = 567,
                Q3NoResponse = 36,
                Q4Hrs0 = 723,
                Q4HrsLt1 = 313,
                Q4Hrs1To2 = 142,
                Q4Hrs2To4 = 60,
                Q4Hrs4To6 = 17,
                Q4HrsGt6 = 25,
                Q4NoResponse = 46,
                Q5No = 488,
                Q5Yes = 756,
                Q5NoResponse = 82,
                Q6NotApplicable = 278,
                Q6No = 436,
                Q6YesIndirectly = 436,
                Q6YesDirectly = 126,
                Q6NoResponse = 50,
                Q7No = 400,
                Q7Yes = 869,
                Q7NoResponse = 57
            };

            // When
            var result = service.GetEvaluationSummaryData(
                121,
                new DateTime(2010, 01, 01),
                new DateTime(2020, 01, 01),
                null,
                null,
                10059
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
