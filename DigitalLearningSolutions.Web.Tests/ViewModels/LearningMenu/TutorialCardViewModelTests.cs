namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class TutorialCardViewModelTests
    {
        private const int CustomisationId = 5;
        private const int SectionId = 5;

        [Test]
        public void Tutorial_card_should_have_average_time()
        {
            // Given
            const int averageTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                averageTutMins: averageTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.AverageTutorialTime.Should().Be(averageTime);
        }

        [Test]
        public void Tutorial_card_should_have_tutorial_time()
        {
            // Given
            const int tutTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutTime: tutTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.TutorialTime.Should().Be(tutTime);
        }

        [Test]
        public void Tutorial_card_should_have_customisation_id()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_card_should_have_section_id()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.SectionId.Should().Be(SectionId);
        }
    }
}
