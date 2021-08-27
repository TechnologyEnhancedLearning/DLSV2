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
        private IActivityService activityService = null!;

        [SetUp]
        public void SetUp()
        {
            activityDataService = A.Fake<IActivityDataService>();
            activityService = new ActivityService(activityDataService);
        }

        [Test]
        [TestCase(ReportInterval.Days, 732, "2014-6-22", "2016-6-22", "2015-12-22")]
        [TestCase(ReportInterval.Months, 25, "2014-6-01", "2016-6-01", "2015-12-01")]
        [TestCase(ReportInterval.Quarters, 9, "2014-4-01", "2016-4-01", "2015-10-01")]
        [TestCase(ReportInterval.Years, 3, "2014-1-01", "2016-1-01", "2015-1-01")]
        public void GetFilteredActivity_correctly_groups_activity(
            ReportInterval interval,
            int expectedSlotCount,
            string expectedStartDate,
            string expectedFinalDate,
            string expectedActionDate
        )
        {
            // given
            var expectedActivityResult = new List<ActivityLog>
            {
                new ActivityLog
                {
                    Completed = true,
                    Evaluated = false,
                    Registered = false,
                    LogDate = DateTime.Parse("2015-12-22"),
                    LogYear = 2015,
                    LogQuarter = 4,
                    LogMonth = 12
                }
            };
            A.CallTo(
                    () => activityDataService.GetFilteredActivity(
                        A<int>._,
                        A<DateTime>._,
                        A<DateTime>._,
                        A<int?>._,
                        A<int?>._,
                        A<int?>._
                    )
                )
                .Returns(expectedActivityResult);

            // when
            var filterData = new ActivityFilterData(
                DateTime.Parse("2014-6-22"),
                DateTime.Parse("2016-6-22"),
                null,
                null,
                null,
                interval
            );
            var result = activityService.GetFilteredActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => activityDataService.GetFilteredActivity(
                            A<int>._,
                            A<DateTime>._,
                            A<DateTime>._,
                            A<int?>._,
                            A<int?>._,
                            A<int?>._
                        )
                    )
                    .MustHaveHappened(1, Times.Exactly);
                result.First().Should().BeEquivalentTo(
                    new PeriodOfActivity(
                        new DateInformation(DateTime.Parse(expectedStartDate), interval),
                        0,
                        0,
                        0
                    )
                );
                result.Last().Should().BeEquivalentTo(
                    new PeriodOfActivity(
                        new DateInformation(DateTime.Parse(expectedFinalDate), interval),
                        0,
                        0,
                        0
                    )
                );
                result.Single(p => p.Completions == 1).Should().BeEquivalentTo(
                    new PeriodOfActivity(
                        new DateInformation(DateTime.Parse(expectedActionDate), interval),
                        0,
                        1,
                        0
                    )
                );
                result.Count.Should().Be(expectedSlotCount);
                result.All(p => p.Evaluations == 0 && p.Registrations == 0).Should().BeTrue();
                result.All(p => p.DateInformation.Interval == interval).Should().BeTrue();
            }
        }

        [Test]
        public void GetFilteredActivity_returns_empty_slots_with_no_activity()
        {
            // when
            var filterData = new ActivityFilterData(
                DateTime.Parse("2115-6-22"),
                DateTime.Parse("2116-9-22"),
                null,
                null,
                null,
                ReportInterval.Months
            );

            var result = activityService.GetFilteredActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => activityDataService.GetFilteredActivity(
                            A<int>._,
                            A<DateTime>._,
                            A<DateTime>._,
                            A<int?>._,
                            A<int?>._,
                            A<int?>._
                        )
                    )
                    .MustHaveHappened(1, Times.Exactly);

                result.Count.Should().Be(16);
                result.All(p => p.Completions == 0 && p.Evaluations == 0 && p.Registrations == 0).Should().BeTrue();
            }
        }

        [Test]
        public void GetFilteredActivity_requests_activity_with_correct_parameters()
        {
            // when
            var filterData = new ActivityFilterData
            (
                DateTime.Parse("2115-6-22"),
                DateTime.Parse("2116-9-22"),
                1,
                2,
                3,
                ReportInterval.Months
            );
            activityService.GetFilteredActivity(101, filterData);

            // then
            A.CallTo(
                    () => activityDataService.GetFilteredActivity(
                        101,
                        filterData.StartDate,
                        filterData.EndDate,
                        filterData.JobGroupId,
                        filterData.CourseCategoryId,
                        filterData.CustomisationId
                    )
                )
                .MustHaveHappened(1, Times.Exactly);
        }
    }
}
