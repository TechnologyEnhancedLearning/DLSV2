namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ActivityServiceTests
    {
        private IActivityDataService activityDataService = null!;
        private IClockService clockService = null!;
        private IActivityService activityService = null!;

        [SetUp]
        public void SetUp()
        {
            activityDataService = A.Fake<IActivityDataService>();
            clockService = A.Fake<IClockService>();
            activityService = new ActivityService(activityDataService);
        }

        [Test]
        public void GetFilteredActivity_gets_correct_activity()
        {
            // given
            var expectedActivityResult = new List<ActivityLog> {new ActivityLog
            {
                Completed = true,
                Evaluated = false,
                Registered = false,
                LogDate = DateTime.Parse("2015-12-22"),
                LogYear = 2015,
                LogQuarter = 4,
                LogMonth = 12
            }};
            A.CallTo(() => activityDataService.GetRawActivity(A<int>._, A<ActivityFilterData>._))
                .Returns(expectedActivityResult);
            GivenCurrentTimeIs(DateTime.Parse("2015-12-22 06:52:09.080"));

            // when
            var filterData = new ActivityFilterData
            {
                StartDate = DateTime.Parse("2015-6-22"),
                EndDate = DateTime.Parse("2016-6-22"),
                ReportInterval = ReportInterval.Months
            };
            var result = activityService.GetFilteredActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                A.CallTo(() => activityDataService.GetRawActivity(A<int>._, A<ActivityFilterData>._))
                    .MustHaveHappened(1, Times.Exactly);
                result.First().Should().BeEquivalentTo(new PeriodOfActivity(
                    new DateInformation { Date = DateTime.Parse("2015-6-01"), Interval = ReportInterval.Months },
                    0,
                    0,
                    0
                ));
                result.Last().Should().BeEquivalentTo(new PeriodOfActivity(
                    new DateInformation { Date = DateTime.Parse("2016-6-01"), Interval = ReportInterval.Months },
                    0,
                    0,
                    0
                ));
                result.Single(p => p.Completions == 1).Should().BeEquivalentTo(new PeriodOfActivity(
                    new DateInformation { Date = DateTime.Parse("2015-12-01"), Interval = ReportInterval.Months },
                    0,
                    1,
                    0
                ));
                result.Count.Should().Be(13);
                result.All(p => p.Evaluations == 0 && p.Registrations == 0).Should().BeTrue();
            }
        }

        private void GivenCurrentTimeIs(DateTime time)
        {
            A.CallTo(() => clockService.UtcNow).Returns(time);
        }
    }
}
