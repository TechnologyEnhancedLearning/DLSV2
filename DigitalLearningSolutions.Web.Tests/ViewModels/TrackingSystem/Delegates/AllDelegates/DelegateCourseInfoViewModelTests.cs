namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateCourseInfoViewModelTests
    {
        private readonly AttemptStats attemptStats = new AttemptStats(0, 0);
        private readonly List<CustomPromptWithAnswer> customPromptsWithAnswers = new List<CustomPromptWithAnswer>();

        [Test]
        public void DelegateCourseInfoViewModel_sets_date_strings_correctly()
        {
            // Given
            var enrolledDate = new DateTime(2021, 05, 1);
            var updatedDate = new DateTime(2021, 05, 2);
            var completeByDate = new DateTime(2021, 05, 3);
            var completedDate = new DateTime(2021, 05, 4);
            var evaluatedDate = new DateTime(2021, 05, 5);
            var info = new DelegateCourseInfo
            {
                Enrolled = enrolledDate,
                LastUpdated = updatedDate,
                CompleteBy = completeByDate,
                Completed = completedDate,
                Evaluated = evaluatedDate,
            };
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

            // Then
            model.Enrolled.Should().Be("01/05/2021");
            model.LastUpdated.Should().Be("02/05/2021");
            model.CompleteBy.Should().Be("03/05/2021");
            model.Completed.Should().Be("04/05/2021");
            model.Evaluated.Should().Be("05/05/2021");
        }

        [TestCase(1, "Self enrolled")]
        [TestCase(2, "Administrator")]
        [TestCase(3, "Group")]
        [TestCase(4, "System")]
        public void DelegateCourseInfoViewModel_sets_enrollment_method_correctly(
            int enrollmentMethodId,
            string enrollmentMethod
        )
        {
            // Given
            var info = new DelegateCourseInfo { EnrolmentMethodId = enrollmentMethodId };
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

            // Then
            model.EnrolmentMethod.Should().Be(enrollmentMethod);
        }

        [TestCase(0, 0, null)]
        [TestCase(0, 1, null)]
        [TestCase(2, 1, "50%")]
        public void DelegateCourseInfoViewModel_sets_pass_rate_correctly(
            int totalAttempts,
            int attemptsPassed,
            string? passRate
        )
        {
            // Given
            var details = new DelegateCourseDetails(
                new DelegateCourseInfo(),
                customPromptsWithAnswers,
                new AttemptStats(totalAttempts, attemptsPassed)
            );

            // When
            var model = new DelegateCourseInfoViewModel(details);

            // Then
            model.PassRateDisplayString.Should().Be(passRate);
        }

        [Test]
        public void DelegateCourseInfoViewModel_without_customisation_name_sets_course_name_correctly()
        {
            // Given
            var info = new DelegateCourseInfo
            {
                ApplicationName = "my application", CustomisationName = "",
            };
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

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
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

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
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

            // Then
            model.Supervisor.Should().BeNull();
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
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

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
            var details = new DelegateCourseDetails(info, customPromptsWithAnswers, attemptStats);

            // When
            var model = new DelegateCourseInfoViewModel(details);

            // Then
            model.Supervisor.Should().Be("firstname surname");
        }
    }
}
