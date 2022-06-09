namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
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
        private const int OneSecondInMs = 1_000;
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
            true,
            EnrollmentDate,
            EnrollmentDate,
            null,
            null,
            null,
            null,
            3,
            "Kevin",
            "Whittaker (Developer)",
            true,
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
            "87487c85-7d35-4b3f-9979-eb734ce90df2",
            101,
            false,
            "HG1",
            false,
            null,
            true
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
            const int progressId = 94323;
            const int candidateId = 1;

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // When
            courseDataService.RemoveCurrentCourse(progressId, candidateId, RemovalMethod.NotRemoved);
            var timeOfRemoval = DateTime.UtcNow;

            // Then
            var progressFields = await connection.GetProgressRemovedFields(progressId);
            progressFields.Item1.Should().Be((int)RemovalMethod.NotRemoved);
            progressFields.Item2.Should().BeCloseTo(timeOfRemoval, OneSecondInMs);
            transaction.Dispose();
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
        public void GetNumberOfActiveCoursesAtCentreFilteredByCategory_excludes_courses_from_archived_applications()
        {
            // When
            var count = courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(101, null);

            // Then
            count.Should().Be(141);
        }

        [Test]
        public void GetNumsOfRecentProgressRecordsForBrand_returns_expected_dict()
        {
            // Given
            var expectedDict = new Dictionary<int, int> { { 206, 9 } };

            // When
            var dict = courseDataService.GetNumsOfRecentProgressRecordsForBrand(
                2,
                new DateTime(2020, 1, 5, 11, 30, 30)
            );

            // Then
            dict.Should().BeEquivalentTo(expectedDict);
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

            result.Should().HaveCount(256);
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
        public void GetDelegateCourseInfosForCourse_should_return_course_info_correctly()
        {
            // When
            var results = courseDataService.GetDelegateCourseInfosForCourse(27915, 101).ToList();

            // Then
            results.Should().HaveCount(34);
            results.Should().ContainEquivalentOf(ExpectedCourseInfo);
        }

        [Test]
        public void GetDelegateCourseInfosForCourse_should_return_course_info_for_all_centres_course()
        {
            // When
            var results = courseDataService.GetDelegateCourseInfosForCourse(27409, 101).ToList();

            // Then
            results.Should().HaveCount(9);
            results[0].CourseName.Should().Be("An Introduction to Cognition - New");
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
            var expectedFirstCourse = new CourseAssessmentDetails
            {
                CustomisationId = 100,
                CentreId = 101,
                ApplicationId = 1,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                CustomisationName = "Standard",
                Active = false,
                CategoryName = "Undefined",
                CourseTopic = "Undefined",
                HasDiagnostic = false,
                HasLearning = true,
                IsAssessed = false,
            };

            result.Should().HaveCount(256);
            result.First().Should().BeEquivalentTo(expectedFirstCourse);
        }

        [Test]
        public void GetCoursesAvailableToCentreByCategory_returns_active_and_inactive_all_centre_courses()
        {
            // Given
            const int centreId = 101;
            const int categoryId = 1;

            var expectedActiveCourse = new CourseAssessmentDetails
            {
                CustomisationId = 17468,
                CentreId = 549,
                ApplicationId = 206,
                ApplicationName = "An Introduction to Cognition",
                CustomisationName = "eLearning",
                Active = true,
                CategoryName = "Undefined",
                CourseTopic = "Undefined",
                HasDiagnostic = false,
                HasLearning = true,
                IsAssessed = false,
            };
            var expectedInactiveCourse = new CourseAssessmentDetails
            {
                CustomisationId = 14738,
                CentreId = 549,
                ApplicationId = 76,
                ApplicationName = "Mobile Directory",
                CustomisationName = "eLearning",
                Active = false,
                CategoryName = "Undefined",
                CourseTopic = "Undefined",
                HasDiagnostic = false,
                HasLearning = true,
                IsAssessed = false,
            };

            // When
            var result = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId).ToList();

            // Then
            result.Should().ContainEquivalentOf(expectedActiveCourse);
            result.Should().ContainEquivalentOf(expectedInactiveCourse);
        }

        [Test]
        public void GetApplicationsAvailableToCentreByCategory_returns_expected_values()
        {
            // Given
            const int centreId = 101;
            int? categoryId = null;

            var expectedFirstApplication = new ApplicationDetails
            {
                ApplicationId = 1,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                PLAssess = false,
                DiagAssess = false,
                CourseTopicId = 3,
                CategoryName = "Office 2007",
                CourseTopic = "Microsoft Office",
            };

            // When
            var result = courseDataService.GetApplicationsAvailableToCentreByCategory(centreId, categoryId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(65);
                result.First().Should().BeEquivalentTo(expectedFirstApplication);
            }
        }

        [Test]
        public void GetApplicationsByBrandId_returns_expected_values()
        {
            // Given
            const int brandId = 1;

            var expectedFirstApplication = new ApplicationDetails()
            {
                ApplicationId = 1,
                ApplicationName = "Entry Level - Win XP, Office 2003/07 OLD",
                PLAssess = false,
                DiagAssess = false,
                CourseTopicId = 3,
                CategoryName = "Office 2007",
                CourseTopic = "Microsoft Office",
                CreatedDate = new DateTime(2015, 9, 7, 12, 18, 11, 810),
            };

            // When
            var result = courseDataService.GetApplicationsByBrandId(brandId).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(50);
                result.First().Should().BeEquivalentTo(expectedFirstApplication);
            }
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
        public void GetCourseValidationDetails_returns_expected_course_validation_details()
        {
            // Given
            var expectedValidationDetails = new CourseValidationDetails
            {
                CentreId = 101,
                CourseCategoryId = 2,
                AllCentres = false,
                CentreHasApplication = true,
            };

            // When
            var validationDetails = courseDataService.GetCourseValidationDetails(100, 101);

            // Then
            validationDetails.Should().BeEquivalentTo(expectedValidationDetails);
        }

        [Test]
        public void GetCourseValidationDetails_returns_null_when_course_does_not_exist()
        {
            // When
            var validationDetails = courseDataService.GetCourseValidationDetails(265, 101);

            // Then
            validationDetails.Should().BeNull();
        }

        [Test]
        public void GetCourseValidationDetails_returns_expected_course_validation_details_for_all_centres_course()
        {
            // Given
            var expectedValidationDetails = new CourseValidationDetails
            {
                CentreId = 549,
                CourseCategoryId = 2,
                AllCentres = true,
                CentreHasApplication = true,
            };

            // When
            var validationDetails = courseDataService.GetCourseValidationDetails(14038, 549);

            // Then
            validationDetails.Should().BeEquivalentTo(expectedValidationDetails);
        }

        [Test]
        public void
            DoesCourseNameExistAtCentre_returns_true_if_course_name_exists_at_centre_other_than_the_customisation_specified()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre("Standard", 101, 1, 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void
            DoesCourseNameExistAtCentre_with_no_customisationId_specified_returns_true_if_course_name_exists_at_centre()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre("Standard", 101, 1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void
            DoesCourseNameExistAtCentre_returns_false_if_course_name_does_not_exist_at_centre_other_than_the_customisation_specified()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre("Standard", 101, 1, 100);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void DoesCourseNameExistAtCentre_returns_false_if_course_name_does_not_exist_at_centre()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre("This course name does not exist", 101, 99);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void DoesCourseNameExistAtCentre_returns_false_if_course_name_does_not_exist_with_applicationId()
        {
            // When
            var result = courseDataService.DoesCourseNameExistAtCentre("Standard", 101, 2, 100);

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

        [Test]
        public void CreateNewCentreCourse_correctly_adds_new_course()
        {
            using var transaction = new TransactionScope();
            try
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
                int? categoryId = null;

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

                // When
                var customisationId = courseDataService.CreateNewCentreCourse(customisation);

                var courseDetails = courseDataService.GetCourseDetailsFilteredByCategory(
                    customisationId,
                    centreId,
                    categoryId
                );

                // Then
                using (new AssertionScope())
                {
                    courseDetails!.CurrentVersion.Should().Be(1);
                    courseDetails.CentreId.Should().Be(centreId);
                    courseDetails.ApplicationId.Should().Be(applicationId);
                    courseDetails.Active.Should().BeTrue();
                    courseDetails.CustomisationName.Should().Be(customisationName);
                    courseDetails.Password.Should().Be(password);
                    courseDetails.SelfRegister.Should().Be(selfRegister);
                    courseDetails.TutCompletionThreshold.Should().Be(tutCompletionThreshold);
                    courseDetails.IsAssessed.Should().Be(isAssessed);
                    courseDetails.DiagCompletionThreshold.Should().Be(diagCompletionThreshold);
                    courseDetails.DiagObjSelect.Should().Be(diagObjSelect);
                    courseDetails.HideInLearnerPortal.Should().Be(hideInLearnerPortal);
                    courseDetails.NotificationEmails.Should().Be(notificationEmails);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetDelegatesOnCourseForExport_returns_expected_values()
        {
            // Given
            var expectedFirstRecord = new CourseDelegateForExport
            {
                IsDelegateActive = true,
                CandidateNumber = "PC97",
                CompleteBy = null,
                DelegateId = 32926,
                DelegateEmail = "erpock.hs@5bntu",
                Enrolled = new DateTime(2012, 07, 02, 13, 30, 37, 807),
                DelegateFirstName = "xxxxx",
                DelegateLastName = "xxxx",
                LastUpdated = new DateTime(2012, 07, 31, 10, 18, 39, 993),
                IsProgressLocked = false,
                ProgressId = 18395,
                RemovedDate = null,
                Completed = null,
                CustomisationId = 1,
                RegistrationAnswer1 = null,
                RegistrationAnswer2 = null,
                RegistrationAnswer3 = null,
                RegistrationAnswer4 = null,
                RegistrationAnswer5 = null,
                RegistrationAnswer6 = null,
                Answer1 = "",
                Answer2 = "",
                Answer3 = "",
                LoginCount = 1,
                Duration = 0,
                DiagnosticScore = 0,
                AllAttempts = 0,
                AttemptsPassed = 0,
            };

            // When
            var result = courseDataService.GetDelegatesOnCourseForExport(1, 2).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(3);
                result.First().Should().BeEquivalentTo(expectedFirstRecord);
            }
        }
    }
}
