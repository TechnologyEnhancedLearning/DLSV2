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
        public void Tutorial_card_should_have_time_information()
        {
            // Given
            const int timeMins = 4;
            const int averageTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutTime: timeMins,
                averageTutMins: averageTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.TimeInformation.Should().Be($"{timeMins}m (average time {averageTime}m)");
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
