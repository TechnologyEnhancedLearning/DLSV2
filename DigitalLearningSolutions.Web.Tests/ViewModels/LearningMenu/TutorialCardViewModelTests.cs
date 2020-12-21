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
        public void Tutorial_card_average_time_spent_string_should_have_plural_if_value_is_not_one()
        {
            // Given
            const int averageTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                averageTutMins: averageTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.AverageTimeInformation.Should().Be($"(average tutorial time {averageTime} minutes)");
        }

        [Test]
        public void Tutorial_card_average_time_spent_string_should_not_have_plural_if_value_is_one()
        {
            // Given
            const int averageTime = 1;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                averageTutMins: averageTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.AverageTimeInformation.Should().Be($"(average tutorial time {averageTime} minute)");
        }

        [Test]
        public void Tutorial_card_time_spent_string_should_have_plural_if_value_is_not_one()
        {
            // Given
            const int tutTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutTime: tutTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.TimeSpentInformation.Should().Be($"{tutTime} minutes spent");
        }

        [Test]
        public void Tutorial_card_time_spent_string_should_not_have_plural_if_value_is_one()
        {
            // Given
            const int tutTime = 1;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutTime: tutTime
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(sectionTutorial, CustomisationId, SectionId);

            // Then
            tutorialCardViewModel.TimeSpentInformation.Should().Be($"{tutTime} minute spent");
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
