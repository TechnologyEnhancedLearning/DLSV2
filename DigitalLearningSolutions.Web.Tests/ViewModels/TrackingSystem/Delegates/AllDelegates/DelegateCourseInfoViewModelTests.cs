namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DelegateCourseInfoViewModelTests
    {
        private readonly AttemptStats attemptStats = new AttemptStats(0, 0);

        private readonly List<CourseAdminFieldWithAnswer> courseAdminFieldsWithAnswers =
            new List<CourseAdminFieldWithAnswer>();

        [Test]
        public void DelegateCourseInfoViewModel_sets_date_strings_correctly()
        {
            // Given
            var enrolledDate = new DateTime(2021, 05, 1, 12, 12, 12);
            var updatedDate = new DateTime(2021, 05, 2, 13, 13, 13);
            var completeByDate = new DateTime(2021, 05, 3, 14, 14, 14);
            var completedDate = new DateTime(2021, 05, 4, 15, 15, 15);
            var evaluatedDate = new DateTime(2021, 05, 5, 16, 16, 16);
            var info = new DelegateCourseInfo
            {
                Enrolled = enrolledDate,
                LastUpdated = updatedDate,
                CompleteBy = completeByDate,
                Completed = completedDate,
                Evaluated = evaluatedDate,
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                new ReturnPageQuery(1, null)
            );

            // Then
            using (new AssertionScope())
            {
                model.Enrolled.Should().Be("01/05/2021 12:12");
                model.LastAccessed.Should().Be("02/05/2021 13:13");
                model.CompleteBy.Should().Be("03/05/2021 14:14");
                model.Completed.Should().Be("04/05/2021 15:15");
                model.Evaluated.Should().Be("05/05/2021 16:16");
            }
        }

        [TestCase(1, "Self enrolled")]
        [TestCase(2, "Enrolled by Test Admin")]
        [TestCase(3, "Group")]
        [TestCase(4, "System")]
        public void DelegateCourseInfoViewModel_sets_enrollment_method_correctly(
            int enrollmentMethodId,
            string enrollmentMethod
        )
        {
            // Given
            var info = new DelegateCourseInfo
            {
                EnrolmentMethodId = enrollmentMethodId,
                EnrolledByForename = "Test",
                EnrolledBySurname = "Admin",
                EnrolledByAdminActive = true,
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.EnrolmentMethod.Should().Be(enrollmentMethod);
        }

        [Test]
        public void DelegateCourseInfoViewModel_without_customisation_name_sets_course_name_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                ApplicationName = "my application",
                CustomisationName = "",
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.CourseName.Should().Be("my application");
        }

        [Test]
        public void DelegateCourseInfoViewModel_with_customisation_name_sets_course_name_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                ApplicationName = "my application",
                CustomisationName = "my customisation",
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.CourseName.Should().Be("my application - my customisation");
        }

        [Test]
        public void DelegateCourseInfoViewModel_without_supervisor_surname_sets_supervisor_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                SupervisorAdminId = null,
                SupervisorSurname = null,
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.Supervisor.Should().Be("None");
        }

        [Test]
        public void DelegateCourseInfoViewModel_without_supervisor_forename_sets_supervisor_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                SupervisorForename = "",
                SupervisorSurname = "surname",
                SupervisorAdminActive = true,
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.Supervisor.Should().Be("surname");
        }

        [Test]
        public void DelegateCourseInfoViewModel_with_supervisor_forename_sets_supervisor_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                SupervisorForename = "firstname",
                SupervisorSurname = "surname",
                SupervisorAdminActive = true,
            };

            // When
            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            model.Supervisor.Should().Be("firstname surname");
        }

        [Test]
        public void DelegateCourseInfoViewModel_with_archived_course_returns_archived_status_tag_style()
        {
            // Given
            var archivedDate = new DateTime(2022, 11, 3, 09, 00, 00);
            var info = new DelegateCourseInfo
            {
                CourseArchivedDate = archivedDate,
            };

            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // When
            var statusTagStyle = model.StatusTagStyle();

            // Then
            statusTagStyle.Should().Be("nhsuk-tag--grey");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DelegateCourseInfoViewModel_with_active_status_course_returns_active_status_tag_style(bool isActive)
        {
            // Given
            var info = new DelegateCourseInfo
            {
                IsCourseActive = isActive,
            };

            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // When
            var statusTagStyle = model.StatusTagStyle();

            // Then
            if (isActive)
            {
                statusTagStyle.Should().Be("nhsuk-tag--green");
            }
            else
            {
                statusTagStyle.Should().Be("nhsuk-tag--red");
            }
        }

        [Test]
        public void DelegateCourseInfoViewModel_with_completed_course_returns_completed_status_tag_style()
        {
            // Given
            var completedDate = new DateTime(2022, 11, 3, 09, 00, 00);
            var info = new DelegateCourseInfo
            {
                Completed = completedDate,
            };

            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // When
            var statusTagStyle = model.StatusTagStyle();

            // Then
            statusTagStyle.Should().Be("nhsuk-tag--red");
        }

        [Test]
        public void DelegateCourseInfoViewModel_with_removed_course_returns_removed_status_tag_style()
        {
            // Given
            var removedDate = new DateTime(2022, 11, 3, 09, 00, 00);
            var info = new DelegateCourseInfo
            {
                RemovedDate = removedDate,
            };

            var model = new DelegateCourseInfoViewModel(
                info,
                DelegateAccessRoute.CourseDelegates,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // When
            var statusTagStyle = model.StatusTagStyle();

            // Then
            statusTagStyle.Should().Be("nhsuk-tag--grey");
        }
    }
}
