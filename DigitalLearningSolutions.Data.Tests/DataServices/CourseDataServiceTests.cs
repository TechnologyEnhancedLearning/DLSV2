namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CourseDataServiceTests
    {
        private static readonly DateTime EnrollmentDate = new DateTime(2019, 04, 11, 14, 33, 37).AddMilliseconds(140);

        private static readonly DelegateCourseInfo ExpectedCourseInfo = new DelegateCourseInfo(
            284998,
            27915,
            101,
            true,
            false,
            4,
            "LinkedIn",
            "Cohort Testing",
            1,
            "Kevin",
            "Whittaker (Developer)",
            EnrollmentDate,
            EnrollmentDate,
            null,
            null,
            null,
            null,
            3,
            1,
            "Kevin",
            "Whittaker (Developer)",
            0,
            0,
            null,
            true,
            null,
            null,
            null,
            20,
            "xxxx",
            "xxxxxx",
            "",
            101
        );

        private SqlConnection connection = null!;
        private CourseDataService courseDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CourseDataService>>();
            courseDataService = new CourseDataService(connection, logger);
        }

        [Test]
        public void Get_current_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 1;
            var result = courseDataService.GetCurrentCourses(candidateId).ToList();

            // Then
            var expectedFirstCourse = new CurrentCourse
            {
                Name = "Office 2013 Essentials for the Workplace - Erin Test 01",
                Id = 15853,
                LastAccessed = new DateTime(2019, 1, 22, 8, 20, 39, 133),
                StartedDate = new DateTime(2016, 7, 6, 11, 12, 15, 393),
                DiagnosticScore = 0,
                IsAssessed = true,
                HasDiagnostic = true,
                HasLearning = true,
                Passes = 2,
                Sections = 6,
                CompleteByDate = new DateTime(2018, 12, 31, 0, 0, 0, 0),
                GroupCustomisationId = 0,
                SupervisorAdminId = 0,
                ProgressID = 173218,
                EnrollmentMethodID = 1,
                PLLocked = false,
            };
            result.Should().HaveCount(4);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_completed_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 1;
            var result = courseDataService.GetCompletedCourses(candidateId).ToList();

            // Then
            var expectedFirstCourse = new CompletedCourse
            {
                Name = "Staying Safe Online Test PLA Issue - test",
                Id = 25140,
                StartedDate = new DateTime(2018, 5, 29, 9, 11, 45, 943),
                Completed = new DateTime(2018, 5, 29, 14, 28, 5, 557),
                LastAccessed = new DateTime(2018, 5, 29, 14, 28, 5, 020),
                Evaluated = new DateTime(2019, 4, 5, 7, 10, 28, 507),
                ArchivedDate = null,
                DiagnosticScore = 0,
                IsAssessed = true,
                HasDiagnostic = true,
                HasLearning = true,
                Passes = 1,
                Sections = 2,
                ProgressID = 251571,
            };
            result.Should().HaveCount(15);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void Get_available_courses_should_return_courses_for_candidate()
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            var expectedFirstCourse = new AvailableCourse
            {
                Name = "5 Jan Test - New",
                Id = 18438,
                Brand = "Local content",
                Topic = "Microsoft Office",
                Category = "Digital Workplace",
                DelegateStatus = 0,
                HasLearning = true,
                HasDiagnostic = true,
                IsAssessed = true,
            };
            result.Should().HaveCountGreaterOrEqualTo(1);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [TestCase(4, "Office 2010")]
        [TestCase(2, null)]
        public void Get_available_courses_should_validate_category(
            int index,
            string? expectedValidatedCategory
        )
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            result[index].Category.Should().Be(expectedValidatedCategory);
        }

        [TestCase(4, "Word")]
        [TestCase(2, null)]
        public void Get_available_courses_should_validate_topic(
            int index,
            string? expectedValidatedTopic
        )
        {
            // When
            const int candidateId = 254480;
            const int centreId = 101;
            var result = courseDataService.GetAvailableCourses(candidateId, centreId).ToList();

            // Then
            result[index].Topic.Should().Be(expectedValidatedTopic);
        }

        [Test]
        public void Get_available_courses_should_return_no_courses_if_no_centre()
        {
            // When
            const int candidateId = 1;
            var result = courseDataService.GetAvailableCourses(candidateId, null).ToList();

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void Set_complete_by_date_should_update_db()
        {
            // Given
            const int candidateId = 1;
            const int progressId = 94323;
            var newCompleteByDate = new DateTime(2020, 7, 29);

            using (new TransactionScope())
            {
                // When
                courseDataService.SetCompleteByDate(progressId, candidateId, newCompleteByDate);
                var modifiedCourse = courseDataService.GetCurrentCourses(candidateId).ToList()
                    .First(c => c.ProgressID == progressId);

                // Then
                modifiedCourse.CompleteByDate.Should().Be(newCompleteByDate);
            }
        }

        [Test]
        public void Remove_current_course_should_prevent_a_course_from_being_returned()
        {
            using (new TransactionScope())
            {
                // Given
                const int progressId = 94323;
                const int candidateId = 1;

                // When
                courseDataService.RemoveCurrentCourse(progressId, candidateId, RemovalMethod.RemovedByDelegate);
                var courseReturned = courseDataService.GetCurrentCourses(candidateId).ToList()
                    .Any(c => c.ProgressID == progressId);

                // Then
                courseReturned.Should().BeFalse();
            }
        }

        [Test]
        public async Task Remove_current_course_sets_removal_date_and_method_correctly()
        {
            // Given
            var removedDate = DateTime.UtcNow;
            const int progressId = 94323;
            const int candidateId = 1;

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // When
                courseDataService.RemoveCurrentCourse(progressId, candidateId, RemovalMethod.NotRemoved);
                var progressFields = await connection.GetProgressRemovedFields(progressId);

                // Then
                progressFields.Item1.Should().Be((int)RemovalMethod.NotRemoved);
                progressFields.Item2.Should().BeCloseTo(removedDate, 500);
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetNumberOfActiveCoursesAtCentreFilteredByCategory_returns_expected_count()
        {
            // When
            var count = courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(2, null);

            // Then
            count.Should().Be(38);
        }

        [Test]
        public void GetNumberOfActiveCoursesAtCentreFilteredByCategory_with_filtered_category_returns_expected_count()
        {
            // When
            var count = courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(2, 2);

            // Then
            count.Should().Be(3);
        }

        [Test]
        public void GetCourseStatisticsAtCentreFilteredByCategory_should_return_course_statistics_correctly()
        {
            // Given
            const int centreId = 101;
            int? categoryId = null;

            // When
            var result = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId).ToList();

            // Then
            var expectedFirstCourse = new CourseStatistics
            {
                CustomisationId = 100,
                CentreId = 101,
                Active = false,
                AllCentres = false,
                ApplicationId = 1,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                CustomisationName = "Standard",
                DelegateCount = 25,
                AllAttempts = 49,
                AttemptsPassed = 34,
                CompletedCount = 5,
                HideInLearnerPortal = false,
                CategoryName = "Office 2007",
                CourseTopic = "Microsoft Office",
                LearningMinutes = "N/A",
            };

            result.Should().HaveCount(260);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void GetCourseDetailsFilteredByCategory_should_return_course_details_correctly()
        {
            // Given
            const int customisationId = 100;
            const int centreId = 101;
            int? categoryId = null;
            var fixedCreationDateTime = DateTime.UtcNow;
            var expectedLastAccess = new DateTime(2014, 03, 31, 13, 00, 23, 457);
            var expectedCourseDetails = CourseDetailsTestHelper.GetDefaultCourseDetails(
                createdDate: fixedCreationDateTime,
                lastAccessed: expectedLastAccess
            );

            // When
            var result =
                courseDataService.GetCourseDetailsFilteredByCategory(customisationId, centreId, categoryId)!;
            // Overwrite the created time as it is populated by a default constraint and not consistent over different databases
            result.CreatedDate = fixedCreationDateTime;

            // Then
            result.Should().BeEquivalentTo(expectedCourseDetails);
        }

        [Test]
        public void GetDelegateCoursesInfo_should_return_delegate_course_info_correctly()
        {
            // When
            var results = courseDataService.GetDelegateCoursesInfo(20).ToList();

            // Then
            results.Should().HaveCount(4);
            results[3].Should().BeEquivalentTo(ExpectedCourseInfo);
        }

        [Test]
        public void GetDelegateCourseInfo_should_return_delegate_course_info_correctly()
        {
            // When
            var result = courseDataService.GetDelegateCourseInfoByProgressId(284998);

            // Then
            result.Should().BeEquivalentTo(ExpectedCourseInfo);
        }

        [Test]
        public void GetDelegateCoursesAttemptStats_should_return_delegate_course_info_correctly()
        {
            // When
            var attemptStats = courseDataService.GetDelegateCourseAttemptStats(11, 100);

            // Then
            attemptStats.TotalAttempts.Should().Be(23);
            attemptStats.AttemptsPassed.Should().Be(11);
            attemptStats.PassRate.Should().Be(48);
        }

        [Test]
        public void GetCourseNameAndApplication_returns_course_name_and_application()
        {
            // When
            var result = courseDataService.GetCourseNameAndApplication(7832);

            // Then
            result?.ApplicationName.Should().Be("Level 2 - Microsoft PowerPoint 2010");
            result?.CustomisationName.Should().Be("PL Testing");
        }

        [Test]
        public void GetCourseNameAndApplication_returns_null_for_nonexistent_course()
        {
            // When
            var result = courseDataService.GetCourseNameAndApplication(-1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCoursesAvailableToCentreByCategory_returns_expected_values()
        {
            // Given
            const int centreId = 101;
            int? categoryId = null;

            // When
            var result = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId).ToList();

            // Then
            var expectedFirstCourse = new Course
            {
                CustomisationId = 100,
                CentreId = 101,
                ApplicationId = 1,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                CustomisationName = "Standard",
                Active = false,
            };

            result.Should().HaveCount(260);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void GetCoursesAvailableToCentreByCategory_returns_active_and_inactive_all_centre_courses()
        {
            // Given
            const int centreId = 101;
            const int categoryId = 1;

            var expectedActiveCourse = new Course
            {
                CustomisationId = 17468,
                CentreId = 549,
                ApplicationId = 206,
                ApplicationName = "An Introduction to Cognition",
                CustomisationName = "eLearning",
                Active = true,
            };
            var expectedInactiveCourse = new Course
            {
                CustomisationId = 14738,
                CentreId = 549,
                ApplicationId = 76,
                ApplicationName = "Mobile Directory",
                CustomisationName = "eLearning",
                Active = false,
            };

            // When
            var result = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId).ToList();

            // Then
            result.Should().ContainEquivalentOf(expectedActiveCourse);
            result.Should().ContainEquivalentOf(expectedInactiveCourse);
        }

        [Test]
        public void GetCoursesEverUsedAtCentreByCategory_returns_courses_no_longer_available_to_centre()
        {
            // Given
            const int centreId = 101;
            int? categoryId = null;

            var expectedUnavailableCourse = new Course
            {
                CustomisationId = 18438,
                CentreId = 101,
                ApplicationId = 301,
                ApplicationName = "5 Jan Test",
                CustomisationName = "New",
                Active = true,
            };

            // When
            var everUsedResult = courseDataService.GetCoursesEverUsedAtCentreByCategory(centreId, categoryId).ToList();

            // Then
            everUsedResult.Should().ContainEquivalentOf(expectedUnavailableCourse);
        }

        [Test]
        public void GetCoursesAvailableToCentreByCategory_does_not_return_unavailable_courses()
        {
            // Given
            const int centreId = 101;
            int? categoryId = null;

            // When
            var availableResult =
                courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId).ToList();

            // Then
            availableResult.Select(c => c.CustomisationId).Should().NotContain(18438);
        }

        [Test]
        public void DoesCourseExistAtCentre_returns_true_if_course_exists()
        {
            // When
            var result = courseDataService.DoesCourseExistAtCentre(100, 101, null);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void DoesCourseExistAtCentre_returns_false_if_course_does_not_exist_at_centre()
        {
            // When
            var result = courseDataService.DoesCourseExistAtCentre(100, 2, 0);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void DoesCourseExistAtCentre_returns_false_if_course_does_not_exist_with_categoryId()
        {
            // When
            var result = courseDataService.DoesCourseExistAtCentre(100, 101, 99);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void GetCourseCentreAndCategory_returns_centreId_and_categoryId_correctly()
        {
            // When
            var (centreId, courseCategoryId) = courseDataService.GetCourseCentreAndCategory(100);

            // Then
            centreId.Should().Be(101);
            courseCategoryId.Should().Be(2);
        }

        [Test]
        public void GetCourseCentreAndCategory_returns_null_when_course_does_not_exist()
        {
            // When
            var (centreId, courseCategoryId) = courseDataService.GetCourseCentreAndCategory(265);

            // Then
            centreId.Should().BeNull();
            courseCategoryId.Should().BeNull();
        }

        [Test]
        public void DoesCourseNameExistAtCentre_returns_true_if_course_name_exists_at_centre()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre(1, "Standard", 101, 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void DoesCourseNameExistAtCentre_returns_false_if_course_name_does_not_exist_at_centre()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre(1, "This course name does not exist", 101, 99);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void DoesCourseNameExistAtCentre_returns_false_if_course_name_does_not_exist_with_applicationId()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre(1, "Standard", 101, 2);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void
            UpdateLearningPathwayDefaultsForCourse_correctly_updates_learning_pathway_defaults_without_auto_refresh()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                courseDataService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, false, 0, 0, false);
                var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                    1,
                    2,
                    null
                );

                // Then
                using (new AssertionScope())
                {
                    courseDetails!.CompleteWithinMonths.Should().Be(6);
                    courseDetails.ValidityMonths.Should().Be(12);
                    courseDetails.Mandatory.Should().Be(true);
                    courseDetails.AutoRefresh.Should().Be(false);
                    courseDetails.RefreshToCustomisationId.Should().Be(0);
                    courseDetails.AutoRefreshMonths.Should().Be(0);
                    courseDetails.ApplyLpDefaultsToSelfEnrol.Should().Be(false);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void
            UpdateLearningPathwayDefaultsForCourse_correctly_updates_learning_pathway_defaults_with_auto_refresh_params()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                courseDataService.UpdateLearningPathwayDefaultsForCourse(1, 6, 12, true, true, 1, 12, true);
                var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                    1,
                    2,
                    null
                );

                // Then
                using (new AssertionScope())
                {
                    courseDetails!.CompleteWithinMonths.Should().Be(6);
                    courseDetails.ValidityMonths.Should().Be(12);
                    courseDetails.Mandatory.Should().Be(true);
                    courseDetails.AutoRefresh.Should().Be(true);
                    courseDetails.RefreshToCustomisationId.Should().Be(1);
                    courseDetails.AutoRefreshMonths.Should().Be(12);
                    courseDetails.ApplyLpDefaultsToSelfEnrol.Should().Be(true);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateCourseDetails_correctly_updates_course_details()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int customisationId = 1;
                const string customisationName = "Name";
                const string password = "Password";
                const string notificationEmails = "hello@test.com";
                const bool isAssessed = true;
                const int tutCompletionThreshold = 0;
                const int diagCompletionThreshold = 0;
                const int centreId = 2;
                int? categoryId = null;

                // When
                courseDataService.UpdateCourseDetails(
                    customisationId,
                    customisationName,
                    password,
                    notificationEmails,
                    isAssessed,
                    tutCompletionThreshold,
                    diagCompletionThreshold
                );

                var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                    customisationId,
                    centreId,
                    categoryId
                );

                // Then
                using (new AssertionScope())
                {
                    courseDetails!.CustomisationName.Should().Be(customisationName);
                    courseDetails.Password.Should().Be(password);
                    courseDetails.NotificationEmails.Should().Be(notificationEmails);
                    courseDetails.IsAssessed.Should().Be(isAssessed);
                    courseDetails.TutCompletionThreshold.Should().Be(tutCompletionThreshold);
                    courseDetails.DiagCompletionThreshold.Should().Be(diagCompletionThreshold);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateCourseOptions_updates_course_options_successfully()
        {
            using var transaction = new TransactionScope();

            // Given
            const int customisationId = 100;
            const int centreId = 101;
            int? categoryId = null;

            var defaultCourseOptions = new CourseOptions
            {
                Active = true,
                DiagObjSelect = true,
                SelfRegister = false,
                HideInLearnerPortal = false,
            };

            try
            {
                // When
                courseDataService.UpdateCourseOptions(defaultCourseOptions, customisationId);
                var updatedCourseOptions = courseDataService.GetCourseOptionsFilteredByCategory(
                    customisationId,
                    centreId,
                    categoryId
                )!;

                // Then
                using (new AssertionScope())
                {
                    updatedCourseOptions.Active.Should().BeTrue();
                    updatedCourseOptions.DiagObjSelect.Should().BeTrue();
                    updatedCourseOptions.SelfRegister.Should().BeFalse();
                    updatedCourseOptions.HideInLearnerPortal.Should().BeFalse();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetCourseOptionsFilteredByCategory_gets_correct_data_for_valid_centre_and_category_Id()
        {
            // Given
            const int customisationId = 1379;
            const int centreId = 101;
            int? categoryId = null;

            // When
            var updatedCourseOptions = courseDataService.GetCourseOptionsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            )!;

            // Then
            using (new AssertionScope())
            {
                updatedCourseOptions.Active.Should().BeTrue();
                updatedCourseOptions.DiagObjSelect.Should().BeTrue();
                updatedCourseOptions.SelfRegister.Should().BeTrue();
                updatedCourseOptions.HideInLearnerPortal.Should().BeTrue();
            }
        }

        [Test]
        public void
            GetCourseOptionsForAdminCategoryId_with_incorrect_centerId_and_correct_customisationId_and_categoryId_returns_null()
        {
            // Given
            const int customisationId = 1379;
            const int centreId = 5;
            int? categoryId = null;

            // When
            var updatedCourseOptions = courseDataService.GetCourseOptionsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            );

            // Then
            updatedCourseOptions.Should().BeNull();
        }

        [Test]
        public void
            GetCourseOptionsForAdminCategoryId_with_incorrect_categoryId_and_correct_customisationId_and_centerId_returns_null()
        {
            // Given
            const int customisationId = 1379;
            const int centreId = 101;
            const int categoryId = 10;

            // When
            var updatedCourseOptions = courseDataService.GetCourseOptionsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            );

            // Then
            updatedCourseOptions.Should().BeNull();
        }
    }
}
