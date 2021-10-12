namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
            var tutorial = TutorialTestHelper.GetDefaultTutorial(status: status);

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
            var tutorial = TutorialTestHelper.GetDefaultTutorial(diagStatus: status);

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.DiagnosticEnabled.Should().BeFalse();
        }

        [Test]
        public void LearningEnabled_should_be_true()
        {
            // Given
            var tutorial = TutorialTestHelper.GetDefaultTutorial();

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.LearningEnabled.Should().BeTrue();
        }

        [Test]
        public void DiagnosticEnabled_should_be_true()
        {
            // Given
            var tutorial = TutorialTestHelper.GetDefaultTutorial();

            // When
            var viewModel = new CourseTutorialViewModel(tutorial);

            // Then
            viewModel.DiagnosticEnabled.Should().BeTrue();
        }
    }
}
