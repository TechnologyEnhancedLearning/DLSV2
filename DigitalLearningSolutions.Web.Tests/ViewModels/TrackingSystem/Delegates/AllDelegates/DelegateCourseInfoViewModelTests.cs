namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateCourseInfoViewModelTests
    {
        private readonly List<CustomPrompt> customPrompts = new List<CustomPrompt>();

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
                Enrolled = enrolledDate, LastUpdated = updatedDate, CompleteBy = completeByDate,
                Completed = completedDate, Evaluated = evaluatedDate
            };

            // When
            var model = new DelegateCourseInfoViewModel(info, customPrompts);

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
            var info = new DelegateCourseInfo { EnrollmentMethodId = enrollmentMethodId };

            // When
            var model = new DelegateCourseInfoViewModel(info, customPrompts);

            // Then
            model.EnrollmentMethod.Should().Be(enrollmentMethod);
        }
    }
}
