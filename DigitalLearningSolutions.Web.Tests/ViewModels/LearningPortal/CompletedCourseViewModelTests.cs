namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CompletedCourseViewModelTests
    {
        private IConfiguration config = null!;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
        }

        [Test]
        public void Completed_course_should_not_have_diagnostic_score_without_diagnostic_assessment()
        {
            // Given
            var completedCourse = CompletedCourseHelper.CreateDefaultCompletedCourse(hasDiagnostic: false);

            // When
            var completedCourseViewModel = new CompletedCourseViewModel(completedCourse, config);

            // Then
            completedCourseViewModel.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Completed_course_should_not_have_diagnostic_score_without_diagnostic_score_value()
        {
            // Given
            var completedCourse = CompletedCourseHelper.CreateDefaultCompletedCourse(diagnosticScore: null);

            // When
            var completedCourseViewModel = new CompletedCourseViewModel(completedCourse, config);

            // Then
            completedCourseViewModel.HasDiagnosticScore().Should().BeFalse();
        }

        [Test]
        public void Completed_course_should_have_diagnostic_score_with_diagnostic_score_value_and_diagnostic_assessment()
        {
            // Given
            var completedCourse = CompletedCourseHelper.CreateDefaultCompletedCourse();

            // When
            var completedCourseViewModel = new CompletedCourseViewModel(completedCourse, config);

            // Then
            completedCourseViewModel.HasDiagnosticScore().Should().BeTrue();
        }

        [Test]
        public void Completed_course_should_not_have_passed_sections_without_learning_assessment()
        {
            // Given
            var completedCourse = CompletedCourseHelper.CreateDefaultCompletedCourse(isAssessed: false);

            // When
            var completedCourseViewModel = new CompletedCourseViewModel(completedCourse, config);

            // Then
            completedCourseViewModel.HasPassedSections().Should().BeFalse();
        }

        [TestCase(false, null, false, "", false)]
        [TestCase(false, null, true, "Certificate", true)]
        [TestCase(false, "1/1/2020", false, "", false)]
        [TestCase(false, "1/1/2020", true, "Certificate", true)]
        [TestCase(true, null, false, "Evaluate", true)]
        [TestCase(true, null, true, "Evaluate", true)]
        [TestCase(true, "1/1/2020", false, "", false)]
        [TestCase(true, "1/1/2020", true, "Certificate", true)]
        public void Completed_course_should_have_correct_finalise_button(
            bool hasLearning,
            DateTime? evaluated,
            bool isAssessed,
            string expectedButtonText,
            bool hasButton
        )
        {
            // Given
            var completedCourse = CompletedCourseHelper.CreateDefaultCompletedCourse(hasLearning: hasLearning, evaluated: evaluated, isAssessed: isAssessed);

            // When
            var completedCourseViewModel = new CompletedCourseViewModel(completedCourse, config);

            // Then
            completedCourseViewModel.FinaliseButtonText().Should().Be(expectedButtonText);
            completedCourseViewModel.HasFinaliseButton().Should().Be(hasButton);
        }
    }
}
