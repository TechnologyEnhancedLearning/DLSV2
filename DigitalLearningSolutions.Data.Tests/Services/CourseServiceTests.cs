namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseServiceTests
    {
        private const int CentreId = 2;
        private const int AdminCategoryId = 0;
        private ICourseDataService courseDataService = null!;
        private CourseService courseService = null!;
        private ICustomPromptsService customPromptsService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            A.CallTo(() => courseDataService.GetCourseStatisticsAtCentreForCategoryId(CentreId, AdminCategoryId))
                .Returns(GetSampleCourses());
            customPromptsService = A.Fake<ICustomPromptsService>();
            courseService = new CourseService(courseDataService, customPromptsService);
        }

        [Test]
        public void GetTopCourseStatistics_should_return_active_course_statistics_ordered_by_InProgress()
        {
            // Given
            var expectedIdOrder = new List<int> { 3, 1 };

            // When
            var resultIdOrder = courseService.GetTopCourseStatistics(CentreId, AdminCategoryId)
                .Select(r => r.CustomisationId).ToList();

            // Then
            Assert.That(resultIdOrder.SequenceEqual(expectedIdOrder));
        }

        [Test]
        public void GetCentreSpecificCourseStatistics_should_only_return_course_statistics_for_centre()
        {
            // Given
            var expectedIdOrder = new List<int> { 1, 2 };

            // When
            var resultIdOrder = courseService.GetCentreSpecificCourseStatistics(CentreId, AdminCategoryId)
                .Select(r => r.CustomisationId).ToList();

            // Then
            resultIdOrder.Should().BeEquivalentTo(expectedIdOrder);
        }

        private IEnumerable<CourseStatistics> GetSampleCourses()
        {
            return new List<CourseStatistics>
            {
                new CourseStatistics
                {
                    CustomisationId = 1,
                    CentreId = CentreId,
                    Active = true,
                    DelegateCount = 100,
                    CompletedCount = 41
                },
                new CourseStatistics
                {
                    CustomisationId = 2,
                    CentreId = CentreId,
                    Active = false,
                    DelegateCount = 50,
                    CompletedCount = 30
                },
                new CourseStatistics
                {
                    CustomisationId = 3,
                    CentreId = CentreId + 1,
                    Active = true,
                    DelegateCount = 500,
                    CompletedCount = 99
                }
            };
        }

        [Test]
        public void GetAllCoursesForDelegate_should_call_correct_data_service_and_helper_methods()
        {
            // Given
            const int delegateId = 20;
            const int customisationId = 111;
            var attemptStats = (7, 4);
            var info = new DelegateCourseInfo
                { CustomisationId = customisationId, IsAssessed = true };
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(delegateId))
                .Returns(new List<DelegateCourseInfo> { info });
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .Returns(attemptStats);

            // When
            var results = courseService.GetAllCoursesForDelegate(delegateId, CentreId).ToList();

            // Then
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(delegateId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () => customPromptsService.GetCustomPromptsWithAnswersForCourse(
                    info,
                    customisationId,
                    CentreId,
                    0
                )
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .MustHaveHappened(1, Times.Exactly);
            results.Should().HaveCount(1);
            results[0].DelegateCourseInfo.Should().BeEquivalentTo(info);
            results[0].AttemptStats.Should().Be(attemptStats);
        }

        [Test]
        public void GetAllCoursesForDelegate_should_not_fetch_attempt_stats_if_course_not_assessed()
        {
            // Given
            const int delegateId = 20;
            const int customisationId = 111;
            var info = new DelegateCourseInfo
                { CustomisationId = customisationId, IsAssessed = false };
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(delegateId))
                .Returns(new List<DelegateCourseInfo> { info });

            // When
            var results = courseService.GetAllCoursesForDelegate(delegateId, CentreId).ToList();

            // Then
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(delegateId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(
                () => customPromptsService.GetCustomPromptsWithAnswersForCourse(
                    info,
                    customisationId,
                    CentreId,
                    0
                )
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(A<int>._, A<int>._)).MustNotHaveHappened();
            results.Should().HaveCount(1);
            results[0].DelegateCourseInfo.Should().BeEquivalentTo(info);
            results[0].AttemptStats.Should().Be((0, 0));
        }
    }
}
