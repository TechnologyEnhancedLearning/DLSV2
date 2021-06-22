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

    class ActivityServiceTests
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
        public void GetRecentActivity_gets_recent_activity_from_one_year_in_december()
        {
            // given
            var expectedActivityResult = new List<MonthOfActivity>{new MonthOfActivity()};
            A.CallTo(() => activityDataService.GetActivityForMonthsInYear(A<int>._, A<int>._, A<IEnumerable<int>>._))
                .Returns(expectedActivityResult);
            GivenCurrentTimeIs(DateTime.Parse("2015-12-22 06:52:09.080"));

            // when
            var result = activityService.GetRecentActivity(101);

            // then
            A.CallTo(() => activityDataService.GetActivityForMonthsInYear(A<int>._, A<int>._, A<IEnumerable<int>>._))
                .MustHaveHappened(1, Times.Exactly);
            result.Should().BeEquivalentTo(expectedActivityResult);
        }

        [Test]
        public void GetRecentActivity_gets_recent_activity_from_two_years_in_earlier_month()
        {
            // given
            var expectedActivityResult = new List<MonthOfActivity> { new MonthOfActivity() };
            A.CallTo(() => activityDataService.GetActivityForMonthsInYear(A<int>._, A<int>._, A<IEnumerable<int>>._))
                .Returns(expectedActivityResult);
            GivenCurrentTimeIs(DateTime.Parse("2015-10-22 06:52:09.080"));

            // when
            var result = activityService.GetRecentActivity(101);

            // then
            A.CallTo(() => activityDataService.GetActivityForMonthsInYear(A<int>._, A<int>._, A<IEnumerable<int>>._))
                .MustHaveHappened(2, Times.Exactly);
            result.Should().BeEquivalentTo(expectedActivityResult.Concat(expectedActivityResult));
        }

        private void GivenCurrentTimeIs(DateTime time)
        {
            A.CallTo(() => clockService.UtcNow).Returns(time);
        }
    }
}
