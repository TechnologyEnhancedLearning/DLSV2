namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
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
                EnrolledByAdminId = null,
                EnrolledByForename = null,
                EnrolledBySurname = null,
                EnrolledByAdminActive = true,
                DelegateId = 1,
            };
            var missingNamesViewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DelegateCourseDetails(
                    missingNamesDelegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                )
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
                EnrolledByAdminId = 1,
                EnrolledByForename = "Geezer",
                EnrolledBySurname = "Butler",
                EnrolledByAdminActive = true,
                DelegateId = 2,
            };
            var fullNamesViewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DelegateCourseDetails(
                    fullNamesDelegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                )
            );

            // Then
            using (new AssertionScope())
            {
                missingNamesViewModel.DelegateName.Should().Be("Osbourne");
                missingNamesViewModel.Supervisor.Should().Be("None");
                missingNamesViewModel.EnrolledByFullName.Should().BeNull();
                fullNamesViewModel.DelegateName.Should().Be("Ozzy Osbourne");
                fullNamesViewModel.Supervisor.Should().Be("Tony Iommi");
                fullNamesViewModel.EnrolledByFullName.Should().Be("Geezer Butler");
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
                new DelegateCourseDetails(
                    delegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                )
            );

            // Then
            viewModel.DelegateName.Should().Be("Bill Ward");
        }

        [Test]
        [TestCase(1, "Self")]
        [TestCase(2, "Enrolled by Ronnie Dio")]
        [TestCase(3, "Group")]
        [TestCase(4, "System")]
        public void ViewModel_sets_Enrolment_method_text_correctly(int enrolmentMethodId, string expectedText)
        {
            // Given
            var delegateInfo = new DelegateCourseInfo
            {
                EnrolmentMethodId = enrolmentMethodId,
                EnrolledByAdminId = 1,
                EnrolledByForename = "Ronnie",
                EnrolledBySurname = "Dio",
                EnrolledByAdminActive = true,
                DelegateId = 1,
            };
            var viewModel = new DelegateProgressViewModel(
                DelegateAccessRoute.ViewDelegate,
                new DelegateCourseDetails(
                    delegateInfo,
                    new List<CourseAdminFieldWithAnswer>(),
                    new AttemptStats(0, 0)
                )
            );

            // Then
            viewModel.EnrolmentMethod.Should().Be(expectedText);
        }
    }
}
