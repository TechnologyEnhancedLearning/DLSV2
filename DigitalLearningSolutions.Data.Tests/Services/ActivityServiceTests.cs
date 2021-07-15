namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
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
            activityService = new ActivityService(activityDataService, clockService);
        }

        [Test]
        public void GetRecentActivity_gets_recent_activity()
        {
            // given
            var expectedActivityResult = new List<MonthOfActivity>{new MonthOfActivity()};
            A.CallTo(() => activityDataService.GetActivityInRangeByMonth(A<int>._, A<DateTime>._, A<DateTime>._))
                .Returns(expectedActivityResult);
            GivenCurrentTimeIs(DateTime.Parse("2015-12-22 06:52:09.080"));

            // when
            var result = activityService.GetRecentActivity(101).ToList();

            // then
            A.CallTo(() => activityDataService.GetActivityInRangeByMonth(A<int>._, A<DateTime>._, A<DateTime>._))
                .MustHaveHappened(1, Times.Exactly);
            result.First().Should().BeEquivalentTo(new MonthOfActivity
            {
                Year = 2014,
                Month = 12,
                Completions = 0,
                Registrations = 0,
                Evaluations = 0
            });
            result.Last().Should().BeEquivalentTo(new MonthOfActivity
            {
                Year = 2015,
                Month = 12,
                Completions = 0,
                Registrations = 0,
                Evaluations = 0
            });
            result.Count().Should().Be(13);
            result.All(m => m.Completions == 0 && m.Evaluations == 0 && m.Registrations == 0).Should().BeTrue();
        }

        private void GivenCurrentTimeIs(DateTime time)
        {
            A.CallTo(() => clockService.UtcNow).Returns(time);
        }
    }
}
