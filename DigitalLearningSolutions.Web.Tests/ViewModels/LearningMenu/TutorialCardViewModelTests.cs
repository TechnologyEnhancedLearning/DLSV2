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
        private const bool ShowTime = true;
        private const bool ShowLearnStatus = true;

        [Test]
        public void Tutorial_card_should_have_timeSummary()
        {
            // Given
            const int averageTutorialDuration = 73;
            const int timeSpent = 41;
            const bool showTime = true;
            const bool showLearnStatus = false;

            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutTime: timeSpent,
                averageTutMins: averageTutorialDuration
            );
            var expectedTimeSummary = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTutorialDuration,
                showTime,
                showLearnStatus
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                showTime,
                showLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.TimeSummary.Should().BeEquivalentTo(expectedTimeSummary);
        }

        [Test]
        public void Tutorial_card_should_show_learning_status_if_showLearnStatus_is_true()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                showTime: true,
                showLearnStatus: true,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowLearnStatus.Should().BeTrue();
        }

        [Test]
        public void Tutorial_card_should_not_show_learning_status_if_showLearnStatus_is_false()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                showTime: true,
                showLearnStatus: false,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowLearnStatus.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_should_have_customisation_id()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_card_should_have_section_id()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.SectionId.Should().Be(SectionId);
        }
    }
}
