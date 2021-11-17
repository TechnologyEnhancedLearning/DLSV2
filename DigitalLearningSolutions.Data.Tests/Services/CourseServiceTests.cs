﻿namespace DigitalLearningSolutions.Data.Tests.Services
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
        private CourseService courseService = null!;
        private IProgressDataService progressDataService = null!;

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
                    CompletedCount = 41,
                },
                new CourseStatistics
                {
                    CustomisationId = 2,
                    CentreId = CentreId,
                    Active = false,
                    DelegateCount = 50,
                    CompletedCount = 30,
                },
                new CourseStatistics
                {
                    CustomisationId = 3,
                    CentreId = CentreId + 1,
                    Active = true,
                    DelegateCount = 500,
                    CompletedCount = 99,
                },
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
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .MustHaveHappenedOnceExactly();
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
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(A<int>._, A<int>._)).MustNotHaveHappened();
            result.DelegateCourseInfo.Should().BeEquivalentTo(info);
            result.AttemptStats.Should().BeEquivalentTo(new AttemptStats(0, 0));
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_return_true_when_centreId_and_categoryId_match()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(A<int>._))
                .Returns((2, 2));

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 2, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void
            VerifyAdminUserCanAccessCourse_should_return_true_when_centreId_matches_and_admin_category_id_is_zero()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(A<int>._))
                .Returns((2, 2));

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 2, 0);

            // Then
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_return_false_with_incorrect_centre()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(A<int>._))
                .Returns((2, 2));

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_return_false_with_incorrect_categoryID()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(A<int>._))
                .Returns((1, 1));

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void RemoveDelegateFromCourse_removes_delegate_from_course()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress> { new Progress { ProgressId = 1, Completed = null, RemovedDate = null } }
            );

            // When
            var result =
                courseService.RemoveDelegateFromCourseIfDelegateHasCurrentProgress(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            result.Should().BeTrue();
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin))
                .MustHaveHappened();
        }

        [Test]
        public void RemoveDelegateFromCourse_returns_false_if_no_current_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>()
            );

            // When
            var result =
                courseService.RemoveDelegateFromCourseIfDelegateHasCurrentProgress(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            result.Should().BeFalse();
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin))
                .MustNotHaveHappened();
        }

        [Test]
        public void VerifyAdminUserCanAccessCourse_should_return_null_when_course_does_not_exist()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(A<int>._))
                .Returns((null, null));

            // When
            var result = courseService.VerifyAdminUserCanAccessCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseCentreAndCategory(1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeNull();
        }

        [Test]
        public void UpdateLearningPathwayDefaultsForCourse_calls_data_service()
        {
            // Given
            A.CallTo(
                () => courseDataService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._
                )
            ).DoesNothing();

            // When
            courseService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, true);

            // Then
            A.CallTo(() => courseDataService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, true))
                .MustHaveHappened();
        }

        [Test]
        public void GetAllCoursesInCategoryForDelegate_filters_courses_by_category()
        {
            // Given
            var info1 = new DelegateCourseInfo { DelegateId = 1, CustomisationId = 1, CourseCategoryId = 1 };
            var info2 = new DelegateCourseInfo { DelegateId = 2, CustomisationId = 2, CourseCategoryId = 1 };
            var info3 = new DelegateCourseInfo { DelegateId = 3, CustomisationId = 3, CourseCategoryId = 2 };
            A.CallTo(
                () => courseDataService.GetDelegateCoursesInfo(1)
            ).Returns(new[] { info1, info2, info3 });

            // When
            var result = courseService.GetAllCoursesInCategoryForDelegate(1, 1, 1).ToList();

            // Then
            result.Count().Should().Be(2);
            result.All(x => x.DelegateCourseInfo.CourseCategoryId == 1).Should().BeTrue();
        }
    }
}
