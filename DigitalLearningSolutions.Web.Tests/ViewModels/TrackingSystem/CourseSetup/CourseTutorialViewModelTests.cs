namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FluentAssertions;
    using NUnit.Framework;

    internal class CourseTutorialViewModelTests
    {
        [Test]
        [TestCase(false)]
        [TestCase(null)]
        public void LearningEnabled_should_be_false_with_false_or_null_tutorial_status(bool status)
        {
            // Given
            var tutorial = new Tutorial
            {
                Status = status
            };

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.LearningEnabled.Should().BeFalse();
        }

        [Test]
        [TestCase(false)]
        [TestCase(null)]
        public void DiagnosticEnabled_should_be_false_with_false_or_null_tutorial_diagnostic_status(bool status)
        {
            // Given
            var tutorial = new Tutorial
            {
                DiagStatus = status
            };

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.DiagnosticEnabled.Should().BeFalse();
        }

        [Test]
        public void LearningEnabled_should_be_true()
        {
            // Given
            var tutorial = new Tutorial
            {
                Status = true
            };

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.LearningEnabled.Should().BeTrue();
        }

        [Test]
        public void DiagnosticEnabled_should_be_true()
        {
            // Given
            var tutorial = new Tutorial
            {
                DiagStatus = true
            };

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.DiagnosticEnabled.Should().BeTrue();
        }
    }
}
