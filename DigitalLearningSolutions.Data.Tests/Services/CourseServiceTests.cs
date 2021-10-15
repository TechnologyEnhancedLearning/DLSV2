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
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDataService courseDataService = null!;
        private CourseService courseService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            A.CallTo(() => courseDataService.GetCourseStatisticsAtCentreForAdminCategoryId(CentreId, AdminCategoryId))
                .Returns(GetSampleCourses());
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            courseService = new CourseService(courseDataService, courseAdminFieldsService);
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
        public void GetDelegateAttemptsAndCourseCustomPrompts_should_call_correct_data_service_and_helper_methods()
        {
            // Given
            const int delegateId = 20;
            const int customisationId = 111;
            var attemptStatsReturnedByDataService = new AttemptStats(10, 5);
            var info = new DelegateCourseInfo
                { DelegateId = delegateId, CustomisationId = customisationId, IsAssessed = true };
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .Returns(attemptStatsReturnedByDataService);

            // When
            var results = courseService.GetDelegateAttemptsAndCourseCustomPrompts(info, CentreId);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
                    info,
                    customisationId,
                    CentreId,
                    false
                )
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .MustHaveHappened(1, Times.Exactly);
            results.DelegateCourseInfo.Should().BeEquivalentTo(info);
            results.AttemptStats.Should().Be(attemptStatsReturnedByDataService);
        }

        [Test]
        public void GetDelegateAttemptsAndCourseCustomPrompts_should_not_fetch_attempt_stats_if_course_not_assessed()
        {
            // Given
            const int customisationId = 111;
            var info = new DelegateCourseInfo
                { CustomisationId = customisationId, IsAssessed = false };

            // When
            var result = courseService.GetDelegateAttemptsAndCourseCustomPrompts(info, CentreId);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
                    info,
                    customisationId,
                    CentreId,
                    false
                )
            ).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(A<int>._, A<int>._)).MustNotHaveHappened();
            result.DelegateCourseInfo.Should().BeEquivalentTo(info);
            result.AttemptStats.Should().BeEquivalentTo(new AttemptStats(0, 0));
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_call_correct_data_service_method()
        {
            // Given
            A.CallTo(() => courseDataService.DoesCourseExistAtCentre(A<int>._, A<int>._, A<int>._))
                .Returns(true);

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 2, 2);

            // Then
            A.CallTo(() => courseDataService.DoesCourseExistAtCentre(A<int>._, A<int>._, A<int>._))
                .MustHaveHappened(1, Times.Exactly);
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_return_return_false_with_incorrect_ids()
        {
            // Given
            A.CallTo(() => courseDataService.DoesCourseExistAtCentre(A<int>._, A<int>._, A<int>._))
                .Returns(false);

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 1, 1);

            // Then
            A.CallTo(() => courseDataService.DoesCourseExistAtCentre(A<int>._, A<int>._, A<int>._))
                .MustHaveHappened(1, Times.Exactly);
            result.Should().BeFalse();
        }
    }
}
