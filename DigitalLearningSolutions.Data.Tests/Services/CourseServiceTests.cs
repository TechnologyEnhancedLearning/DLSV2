namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseServiceTests
    {
        private const int CentreId = 2;
        private const int AdminCategoryId = 0;
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseCategoriesDataService courseCategoriesDataService = null!;
        private ICourseDataService courseDataService = null!;
        private CourseService courseService = null!;
        private ICourseTopicsDataService courseTopicsDataService = null!;
        private IGroupsDataService groupsDataService = null!;
        private IProgressDataService progressDataService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            A.CallTo(() => courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(CentreId, AdminCategoryId))
                .Returns(GetSampleCourses());
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            progressDataService = A.Fake<IProgressDataService>();
            groupsDataService = A.Fake<IGroupsDataService>();
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            courseTopicsDataService = A.Fake<ICourseTopicsDataService>();
            courseService = new CourseService(
                courseDataService,
                courseAdminFieldsService,
                progressDataService,
                groupsDataService,
                courseCategoriesDataService,
                courseTopicsDataService
            );
        }

        [Test]
        public void GetTopCourseStatistics_should_return_active_course_statistics_ordered_by_InProgress()
        {
            // Given
            var expectedIdOrder = new List<int> { 3, 4, 1 };

            // When
            var resultIdOrder = courseService.GetTopCourseStatistics(CentreId, AdminCategoryId)
                .Select(r => r.CustomisationId).ToList();

            // Then
            Assert.That(resultIdOrder.SequenceEqual(expectedIdOrder));
        }

        [Test]
        public void
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts_should_only_return_course_statistics_for_centre()
        {
            // Given
            var expectedIdOrder = new List<int> { 1, 2 };

            // When
            var resultIdOrder = courseService
                .GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(CentreId, AdminCategoryId)
                .Select(r => r.CustomisationId).ToList();

            // Then
            resultIdOrder.Should().BeEquivalentTo(expectedIdOrder);
        }

        [Test]
        public void
            GetAllCourseStatisticsWithAdminFieldResponseCounts_should_return_course_statistics_for_centre_and_all_centre_courses()
        {
            // Given
            var expectedIdOrder = new List<int> { 1, 2, 4 };

            // When
            var resultIdOrder = courseService
                .GetAllCourseStatisticsWithAdminFieldResponseCounts(CentreId, AdminCategoryId)
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
                    AllCentres = false,
                },
                new CourseStatistics
                {
                    CustomisationId = 2,
                    CentreId = CentreId,
                    Active = false,
                    DelegateCount = 50,
                    CompletedCount = 30,
                    AllCentres = false,
                },
                new CourseStatistics
                {
                    CustomisationId = 3,
                    CentreId = CentreId + 1,
                    Active = true,
                    DelegateCount = 500,
                    CompletedCount = 99,
                    AllCentres = false,
                },
                new CourseStatistics
                {
                    CustomisationId = 4,
                    CentreId = 5,
                    Active = true,
                    DelegateCount = 500,
                    CompletedCount = 134,
                    AllCentres = true,
                },
            };
        }

        [Test]
        public void GetDelegateAttemptsAndCourseAdminFields_should_call_correct_data_service_and_helper_methods()
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
            var results = courseService.GetDelegateAttemptsAndCourseAdminFields(info);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                    info,
                    customisationId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(delegateId, customisationId))
                .MustHaveHappenedOnceExactly();
            results.DelegateCourseInfo.Should().BeEquivalentTo(info);
            results.AttemptStats.Should().Be(attemptStatsReturnedByDataService);
        }

        [Test]
        public void GetDelegateAttemptsAndCourseAdminFields_should_not_fetch_attempt_stats_if_course_not_assessed()
        {
            // Given
            const int customisationId = 111;
            var info = new DelegateCourseInfo
                { CustomisationId = customisationId, IsAssessed = false };

            // When
            var result = courseService.GetDelegateAttemptsAndCourseAdminFields(info);

            // Then
            A.CallTo(
                () => courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                    info,
                    customisationId
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => courseDataService.GetDelegateCourseAttemptStats(A<int>._, A<int>._)).MustNotHaveHappened();
            result.DelegateCourseInfo.Should().BeEquivalentTo(info);
            result.AttemptStats.Should().BeEquivalentTo(new AttemptStats(0, 0));
        }

        [Test]
        public void VerifyAdminUserCanManageCourse_should_return_true_when_centreId_and_categoryId_match()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 2,
                CourseCategoryId = 2,
                AllCentres = false,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 2, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 2))
                .MustHaveHappenedTwiceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void
            VerifyAdminUserCanManageCourse_should_return_true_when_centreId_matches_and_admin_category_id_is_null()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 2,
                CourseCategoryId = 2,
                AllCentres = false,
                CentreHasApplication = true,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 2, null);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 2))
                .MustHaveHappenedTwiceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanManageCourse_should_return_false_with_all_centres_course_on_different_centre()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 2,
                CourseCategoryId = 2,
                AllCentres = true,
                CentreHasApplication = true,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedTwiceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void VerifyAdminUserCanManageCourse_should_return_true_with_all_centres_course_on_same_centre()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 2,
                CourseCategoryId = 2,
                AllCentres = true,
                CentreHasApplication = true,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 2, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 2))
                .MustHaveHappenedTwiceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanManageCourse_should_return_false_with_incorrect_categoryID()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 1,
                AllCentres = true,
                CentreHasApplication = true,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void VerifyAdminUserCanManageCourse_should_return_null_when_course_does_not_exist()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = courseService.VerifyAdminUserCanManageCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeNull();
        }

        [Test]
        public void VerifyAdminUserCanViewCourse_should_return_null_when_course_does_not_exist()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeNull();
        }

        [Test]
        public void VerifyAdminUserCanViewCourse_should_return_true_when_course_is_not_at_centre_but_is_all_centres()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 2,
                CourseCategoryId = 2,
                AllCentres = true,
                CentreHasApplication = true,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanViewCourse_should_return_true_when_course_is_at_centre_but_is_not_all_centres()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 2,
                AllCentres = false,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void
            VerifyAdminUserCanViewCourse_should_return_true_when_course_is_at_centre_and_all_centres_without_application()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 2,
                AllCentres = true,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyAdminUserCanViewCourse_should_return_false_with_incorrect_categoryID()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 1,
                AllCentres = false,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, 2);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void
            VerifyAdminUserCanViewCourse_should_return_true_when_centreId_matches_and_admin_category_id_is_null()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 1,
                AllCentres = false,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 1, null);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 1))
                .MustHaveHappenedOnceExactly();
            result.Should().BeTrue();
        }

        [Test]
        public void
            VerifyAdminUserCanViewCourse_should_return_false_when_course_is_not_at_centre_and_is_not_all_centres()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 1,
                AllCentres = false,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 2, null);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 2))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void
            VerifyAdminUserCanViewCourse_should_return_false_when_course_is_all_centres_but_centre_doesnt_have_access()
        {
            // Given
            var validationDetails = new CourseValidationDetails
            {
                CentreId = 1,
                CourseCategoryId = 1,
                AllCentres = true,
                CentreHasApplication = false,
            };
            A.CallTo(() => courseDataService.GetCourseValidationDetails(A<int>._, A<int>._))
                .Returns(validationDetails);

            // When
            var result = courseService.VerifyAdminUserCanViewCourse(1, 2, null);

            // Then
            A.CallTo(() => courseDataService.GetCourseValidationDetails(1, 2))
                .MustHaveHappenedOnceExactly();
            result.Should().BeFalse();
        }

        [Test]
        public void DelegateHasCurrentProgress_returns_true_if_delegate_has_current_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>
                {
                    new Progress { ProgressId = 1, Completed = null, RemovedDate = null },
                    new Progress { ProgressId = 1, Completed = DateTime.UtcNow, RemovedDate = null },
                    new Progress { ProgressId = 1, Completed = null, RemovedDate = DateTime.UtcNow },
                }
            );

            // When
            var result = courseService.DelegateHasCurrentProgress(1, 1);

            // then
            result.Should().BeTrue();
        }

        [Test]
        public void DelegateHasCurrentProgress_returns_false_if_delegate_has_no_current_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>()
            );

            // When
            var result = courseService.DelegateHasCurrentProgress(1, 1);

            // then
            result.Should().BeFalse();
        }

        [Test]
        public void DelegateHasCurrentProgress_returns_false_if_delegate_has_only_completed_or_removed_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>
                {
                    new Progress { ProgressId = 1, Completed = DateTime.UtcNow, RemovedDate = null },
                    new Progress { ProgressId = 1, Completed = null, RemovedDate = DateTime.UtcNow },
                }
            );

            // When
            var result = courseService.DelegateHasCurrentProgress(1, 1);

            // then
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
            courseService.RemoveDelegateFromCourse(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin))
                .MustHaveHappened();
        }

        [Test]
        public void RemoveDelegateFromCourse_does_nothing_if_no_current_progress()
        {
            // Given
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(1, 1)).Returns(
                new List<Progress>()
            );

            // When
            courseService.RemoveDelegateFromCourse(1, 1, RemovalMethod.RemovedByAdmin);

            // then
            A.CallTo(() => courseDataService.RemoveCurrentCourse(1, 1, RemovalMethod.RemovedByAdmin))
                .MustNotHaveHappened();
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
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).DoesNothing();

            // When
            courseService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, true);

            // Then
            A.CallTo(() => courseDataService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, true, 0, 0, false))
                .MustHaveHappened();
        }

        [Test]
        public void GetCourseOptionAlphabeticalListForCentre_calls_data_service()
        {
            // Given
            const int categoryId = 1;
            const int centreId = 1;
            var courseOptions = new List<CourseAssessmentDetails>();
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(courseOptions);

            // When
            var result = courseService.GetCourseOptionsAlphabeticalListForCentre(centreId, categoryId);

            // Then
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .MustHaveHappened();
            result.Should().BeEquivalentTo(courseOptions);
        }

        [Test]
        public void GetApplicationOptionsAlphabeticalListForCentre_calls_data_service()
        {
            // Given
            const int categoryId = 1;
            const int centreId = 1;
            var applicationOptions = new List<ApplicationDetails>();
            A.CallTo(() => courseDataService.GetApplicationsAvailableToCentreByCategory(centreId, categoryId))
                .Returns(applicationOptions);

            // When
            var result = courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => courseDataService.GetApplicationsAvailableToCentreByCategory(centreId, categoryId))
                    .MustHaveHappenedOnceExactly();
                result.Should().BeEquivalentTo(applicationOptions);
            }
        }

        [Test]
        public void GetApplicationOptionsAlphabeticalListForCentre_filters_by_topic_if_topic_given()
        {
            // Given
            const int categoryId = 1;
            const int centreId = 1;
            const int topicId = 1;

            var applicationOne = new ApplicationDetails { CourseTopicId = 1 };
            var applicationTwo = new ApplicationDetails { CourseTopicId = 1 };
            var applicationThree = new ApplicationDetails { CourseTopicId = 2 };

            var applicationOptions = new List<ApplicationDetails>
                { applicationOne, applicationTwo, applicationThree };
            A.CallTo(() => courseDataService.GetApplicationsAvailableToCentreByCategory(centreId, categoryId))
                .Returns(applicationOptions);

            // When
            var result = courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId, topicId)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().OnlyContain(a => a.CourseTopicId == 1);
                result.Count.Should().Be(2);
            }
        }

        [Test]
        public void DoesCourseNameExistAtCentre_calls_data_service()
        {
            // Given
            const int customisationId = 1;
            const string customisationName = "Name";
            const int centreId = 101;
            const int applicationId = 1;

            // When
            courseService.DoesCourseNameExistAtCentre(customisationName, centreId, applicationId, customisationId);

            // Then
            A.CallTo(
                    () => courseDataService.DoesCourseNameExistAtCentre(
                        customisationName,
                        centreId,
                        applicationId,
                        customisationId
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void UpdateCourseDetails_calls_data_service()
        {
            // Given
            const int customisationId = 1;
            const string customisationName = "Name";
            const string password = "Password";
            const string notificationEmails = "hello@test.com";
            const bool isAssessed = true;
            const int tutCompletionThreshold = 0;
            const int diagCompletionThreshold = 0;

            A.CallTo(
                () => courseDataService.UpdateCourseDetails(
                    customisationId,
                    customisationName,
                    password,
                    notificationEmails,
                    isAssessed,
                    tutCompletionThreshold,
                    diagCompletionThreshold
                )
            ).DoesNothing();

            // When
            courseService.UpdateCourseDetails(
                customisationId,
                customisationName,
                password,
                notificationEmails,
                isAssessed,
                tutCompletionThreshold,
                diagCompletionThreshold
            );

            // Then
            A.CallTo(
                    () => courseDataService.UpdateCourseDetails(
                        customisationId,
                        customisationName,
                        password,
                        notificationEmails,
                        isAssessed,
                        tutCompletionThreshold,
                        diagCompletionThreshold
                    )
                )
                .MustHaveHappened();
        }

        [Test]
        public void GetAllCoursesForDelegate_returns_only_courses_at_centre_or_all_centres_courses()
        {
            // Given
            const int delegateId = 1;
            const int centreId = 1;
            const int categoryId = 1;
            var delegateCourseInfoAtCentre = new DelegateCourseInfo
                { CustomisationCentreId = centreId, CourseCategoryId = categoryId };
            var delegateCourseInfoNotAtCentre = new DelegateCourseInfo
                { CustomisationCentreId = 1000, CourseCategoryId = categoryId };
            var allCentresCourseInfoNotAtCentre = new DelegateCourseInfo
                { CustomisationCentreId = 1000, CourseCategoryId = categoryId, AllCentresCourse = true };
            A.CallTo(() => courseDataService.GetDelegateCoursesInfo(delegateId))
                .Returns(
                    new[] { delegateCourseInfoAtCentre, delegateCourseInfoNotAtCentre, allCentresCourseInfoNotAtCentre }
                );

            // When
            var result = courseService.GetAllCoursesInCategoryForDelegate(delegateId, centreId, categoryId).ToList();

            // Then
            result.Count.Should().Be(2);
            result.All(
                x => x.DelegateCourseInfo.CustomisationCentreId == centreId || x.DelegateCourseInfo.AllCentresCourse
            ).Should().BeTrue();
        }

        [Test]
        public void GetAllCoursesInCategoryForDelegate_filters_courses_by_category()
        {
            // Given
            var info1 = new DelegateCourseInfo
                { DelegateId = 1, CustomisationId = 1, CourseCategoryId = 1, CustomisationCentreId = 1 };
            var info2 = new DelegateCourseInfo
                { DelegateId = 2, CustomisationId = 2, CourseCategoryId = 1, CustomisationCentreId = 1 };
            var info3 = new DelegateCourseInfo
                { DelegateId = 3, CustomisationId = 3, CourseCategoryId = 2, CustomisationCentreId = 1 };
            A.CallTo(
                () => courseDataService.GetDelegateCoursesInfo(1)
            ).Returns(new[] { info1, info2, info3 });

            // When
            var result = courseService.GetAllCoursesInCategoryForDelegate(1, 1, 1).ToList();

            // Then
            result.Count.Should().Be(2);
            result.All(x => x.DelegateCourseInfo.CourseCategoryId == 1).Should().BeTrue();
        }

        [Test]
        public void GetEligibleCoursesToAddToGroup_does_not_return_inactive_courses()
        {
            // Given
            const int centreId = 1;
            const int categoryId = 1;
            const int groupId = 1;
            var courses = Builder<CourseAssessmentDetails>.CreateListOfSize(5)
                .All()
                .With(c => c.Active = false)
                .Build();
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(courses);
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(centreId))
                .Returns(new List<GroupCourse>());

            // When
            var result = courseService.GetEligibleCoursesToAddToGroup(centreId, categoryId, groupId);

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void GetEligibleCoursesToAddToGroup_does_not_return_courses_already_in_group()
        {
            // Given
            const int centreId = 1;
            const int categoryId = 1;
            const int groupId = 1;
            var courses = Builder<CourseAssessmentDetails>.CreateListOfSize(5)
                .All()
                .With(c => c.Active = true)
                .Build();
            var groupCourse = new GroupCourse { CustomisationId = 2, Active = true, GroupId = 1 };
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(courses);
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(centreId))
                .Returns(new List<GroupCourse> { groupCourse });

            // When
            var result = courseService.GetEligibleCoursesToAddToGroup(centreId, categoryId, groupId).ToList();

            // Then
            result.Should().HaveCount(4);
            result.Should().NotContain(c => c.CustomisationId == 2);
        }

        [Test]
        public void GetCategoriesForCentreAndCentrallyManagedCourses_returns_expected_categories()
        {
            // Given
            const string categoryName = "Category";
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(CentreId))
                .Returns(new List<Category> { new Category { CourseCategoryID = 1, CategoryName = categoryName } });

            // When
            var result = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(CentreId);

            // Then
            result.Single().Should().Be(categoryName);
        }

        [Test]
        public void GetTopicsForCentreAndCentrallyManagedCourses_returns_expected_topics()
        {
            // Given
            const string topicName = "Topic";
            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(CentreId))
                .Returns(new List<Topic> { new Topic { CourseTopicID = 1, CourseTopic = topicName } });

            // When
            var result = courseService.GetTopicsForCentreAndCentrallyManagedCourses(CentreId);

            // Then
            result.Single().Should().Be(topicName);
        }

        [Test]
        public void CreateNewCentreCourse_calls_data_service()
        {
            // Given
            const int centreId = 2;
            const int applicationId = 1;
            const string customisationName = "Name";
            const string password = "Password";
            const bool selfRegister = false;
            const int tutCompletionThreshold = 0;
            const bool isAssessed = true;
            const int diagCompletionThreshold = 0;
            const bool diagObjSelect = false;
            const bool hideInLearnerPortal = false;
            const string notificationEmails = "hello@test.com";

            var customisation = new Customisation(
                centreId,
                applicationId,
                customisationName,
                password,
                selfRegister,
                tutCompletionThreshold,
                isAssessed,
                diagCompletionThreshold,
                diagObjSelect,
                hideInLearnerPortal,
                notificationEmails
            );

            A.CallTo(
                () => courseDataService.CreateNewCentreCourse(customisation)
            ).Returns(123);

            // When
            var result = courseService.CreateNewCentreCourse(customisation);

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(123);
                A.CallTo(
                    () => courseDataService.CreateNewCentreCourse(customisation)
                ).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetLearningLogDetails_returns_null_when_course_details_cannot_be_found()
        {
            // Given
            const int progressId = 1;
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(null);

            // When
            var result = courseService.GetLearningLogDetails(progressId);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => progressDataService.GetLearningLogEntries(progressId)
                ).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetLearningLogDetails_returns_course_and_log_details_when_available()
        {
            // Given
            const int progressId = 1;
            var courseDetails = Builder<DelegateCourseInfo>.CreateNew().Build();
            var learningLogEntries = Builder<LearningLogEntry>.CreateListOfSize(5).Build();
            var expectedLearningLog = new LearningLog(courseDetails, learningLogEntries);
            A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId)).Returns(courseDetails);
            A.CallTo(() => progressDataService.GetLearningLogEntries(progressId)).Returns(learningLogEntries);

            // When
            var result = courseService.GetLearningLogDetails(progressId);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedLearningLog);
                A.CallTo(() => courseDataService.GetDelegateCourseInfoByProgressId(progressId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => progressDataService.GetLearningLogEntries(progressId)
                ).MustHaveHappenedOnceExactly();
            }
        }
    }
}
