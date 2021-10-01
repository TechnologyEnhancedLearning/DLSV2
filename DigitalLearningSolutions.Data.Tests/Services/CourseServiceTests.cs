namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
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
        private IProgressDataService progressDataService = null!;
        private CourseService courseService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            A.CallTo(() => courseDataService.GetCourseStatisticsAtCentreForAdminCategoryId(CentreId, AdminCategoryId))
                .Returns(GetSampleCourses());
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            progressDataService = A.Fake<IProgressDataService>();
            courseService = new CourseService(courseDataService, courseAdminFieldsService, progressDataService);
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
                () => courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
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
                () => courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
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

        [Test]
        public void RemoveDelegateFromCourse_removes_delegate_from_course()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );

            // When
            var result = courseService.RemoveDelegateFromCourse(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            result.Should().BeTrue();
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin)).MustHaveHappened();
        }

        [Test]
        public void RemoveDelegateFromCourse_returns_false_if_no_current_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>()
            );

            // When
            var result = courseService.RemoveDelegateFromCourse(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            result.Should().BeFalse();
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin)).MustNotHaveHappened();
        }
    }
}
