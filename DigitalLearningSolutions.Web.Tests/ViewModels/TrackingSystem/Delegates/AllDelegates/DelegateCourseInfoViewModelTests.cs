﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
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
        private readonly (int totalAttempts, int attemptsPassed) attemptStats = (0, 0);
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
                Enrolled = enrolledDate, LastUpdated = updatedDate, CompleteBy = completeByDate,
                Completed = completedDate, Evaluated = evaluatedDate
            };

            // When
            var model = new DelegateCourseInfoViewModel(info, customPromptsWithAnswers, attemptStats);

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
            var model = new DelegateCourseInfoViewModel(info, customPromptsWithAnswers, attemptStats);

            // Then
            model.EnrollmentMethod.Should().Be(enrollmentMethod);
        }

        [TestCase(0, 0, null)]
        [TestCase(0, 1, null)]
        [TestCase(2, 1, 50)]
        [TestCase(3, 1, 33)]
        [TestCase(4, 1, 25)]
        [TestCase(100, 1, 1)]
        public void DelegateCourseInfoViewModel_sets_pass_rate_correctly(
            int totalAttempts,
            int attemptsPassed,
            double? passRate
        )
        {
            // When
            var model = new DelegateCourseInfoViewModel(
                new DelegateCourseInfo(),
                customPromptsWithAnswers,
                (totalAttempts, attemptsPassed)
            );

            // Then
            model.PassRate.Should().Be(passRate);
        }
    }
}
