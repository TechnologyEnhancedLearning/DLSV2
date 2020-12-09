namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class TutorialViewModelTests
    {
        private const int CustomisationId = 1;
        private const int SectionId = 10;

        [Test]
        public void Tutorial_should_have_tutorial_content()
        {
            // Given
            var expectedCourseContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var initialMenuViewModel = new TutorialViewModel(expectedCourseContent, CustomisationId, SectionId);

            // Then
            initialMenuViewModel.TutorialContent.Should().BeEquivalentTo(expectedCourseContent);
        }

        [Test]
        public void Tutorial_should_have_customisationId()
        {
            // Given
            var expectedCourseContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var tutorialViewModel = new TutorialViewModel(expectedCourseContent, CustomisationId, SectionId);

            // Then
            tutorialViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_should_have_sectionId()
        {
            // Given
            var expectedCourseContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var tutorialViewModel = new TutorialViewModel(expectedCourseContent, CustomisationId, SectionId);

            // Then
            tutorialViewModel.SectionId.Should().Be(SectionId);
        }
    }
}
