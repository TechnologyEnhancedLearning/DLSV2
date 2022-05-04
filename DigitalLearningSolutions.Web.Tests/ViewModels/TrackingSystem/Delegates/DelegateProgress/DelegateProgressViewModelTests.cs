namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DelegateProgressViewModelTests
    {
        [Test]
        public void ViewModel_sets_full_names_correctly()
        {
            // Given
            var missingNamesDelegateInfo = new DelegateCourseInfo
            {
                DelegateFirstName = null,
                DelegateLastName = "Osbourne",
                DelegateEmail = "Prince@Darkness",
                SupervisorAdminId = null,
                SupervisorForename = null,
                SupervisorSurname = null,
                SupervisorAdminActive = true,
                EnrolledByForename = null,
                EnrolledBySurname = null,
                EnrolledByAdminActive = true,
                DelegateId = 1,
            };
            var missingNamesViewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DetailedCourseProgress(
                    new Progress(),
                    new List<DetailedSectionProgress>(),
                    missingNamesDelegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                ),
                "www.test.com",
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );
            var fullNamesDelegateInfo = new DelegateCourseInfo
            {
                DelegateFirstName = "Ozzy",
                DelegateLastName = "Osbourne",
                DelegateEmail = "Prince@Darkness",
                SupervisorAdminId = 1,
                SupervisorForename = "Tony",
                SupervisorSurname = "Iommi",
                SupervisorAdminActive = true,
                EnrolledByForename = "Geezer",
                EnrolledBySurname = "Butler",
                EnrolledByAdminActive = true,
                DelegateId = 2,
            };
            var fullNamesViewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DetailedCourseProgress(
                    new Progress(),
                    new List<DetailedSectionProgress>(),
                    fullNamesDelegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                ),
                "www.test.com",
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            using (new AssertionScope())
            {
                missingNamesViewModel.DelegateName.Should().Be("Osbourne");
                missingNamesViewModel.Supervisor.Should().Be("None");
                fullNamesViewModel.DelegateName.Should().Be("Ozzy Osbourne");
                fullNamesViewModel.Supervisor.Should().Be("Tony Iommi");
            }
        }

        [Test]
        public void ViewModel_does_not_include_email_if_not_set()
        {
            // Given
            var delegateInfo = new DelegateCourseInfo
            {
                DelegateFirstName = "Bill",
                DelegateLastName = "Ward",
                DelegateEmail = null,
                DelegateId = 1,
            };
            var viewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DetailedCourseProgress(
                    new Progress(),
                    new List<DetailedSectionProgress>(),
                    delegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                ),
                "www.test.com",
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            viewModel.DelegateName.Should().Be("Bill Ward");
        }

        [Test]
        [TestCase(1, "Self enrolled")]
        [TestCase(2, "Enrolled by Ronnie Dio")]
        [TestCase(3, "Group")]
        [TestCase(4, "System")]
        public void ViewModel_sets_Enrolment_method_text_correctly(int enrolmentMethodId, string expectedText)
        {
            // Given
            var delegateInfo = new DelegateCourseInfo
            {
                EnrolmentMethodId = enrolmentMethodId,
                EnrolledByForename = "Ronnie",
                EnrolledBySurname = "Dio",
                EnrolledByAdminActive = true,
                DelegateId = 1,
            };
            var viewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DetailedCourseProgress(
                    new Progress(),
                    new List<DetailedSectionProgress>(),
                    delegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                ),
                "www.test.com",
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            viewModel.EnrolmentMethod.Should().Be(expectedText);
        }
    }
}
