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
        private const bool DiagnosticStatus = true;
        private const int DiagnosticAttempts = 1;

        [TestCase(0, 1, true, true)]
        [TestCase(1, 30, true, false)]
        [TestCase(30, 120, false, true)]
        [TestCase(120, 61, false, false)]
        [TestCase(61, 195, true, true)]
        [TestCase(195, 0, true, false)]
        public void Tutorial_card_should_have_timeSummary(
            int timeSpent,
            int averageTutorialDuration,
            bool showTime,
            bool showLearnStatus
        )
        {
            // Given
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

        [Test]
        public void Tutorial_card_should_show_recommendation_status_if_all_conditions_are_met()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutorialDiagnosticStatus: true,
                tutorialDiagnosticAttempts: 2
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                showLearnStatus: true,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowRecommendationStatus.Should().BeTrue();
        }

        [Test]
        public void Tutorial_card_should_not_show_recommendation_status_if_diagnostic_status_is_false()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutorialDiagnosticStatus: false,
                tutorialDiagnosticAttempts: 2
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowRecommendationStatus.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_should_not_show_recommendation_status_if_show_learn_status_is_false()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                showLearnStatus: false,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowRecommendationStatus.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_should_not_show_recommendation_status_if_diagnostic_attempts_is_zero()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                tutorialDiagnosticAttempts: 0,
                tutorialDiagnosticStatus: true
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowRecommendationStatus.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_recommendation_status_should_be_optional_if_current_score_equals_possible_score()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                currentScore: 10,
                possibleScore: 10
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.RecommendationStatus.Should().Be("Optional");
        }

        [Test]
        public void Tutorial_card_recommendation_status_should_be_recommended_if_current_score_is_less_than_possible_score()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                currentScore: 7,
                possibleScore: 10
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.RecommendationStatus.Should().Be("Recommended");
        }

        [Test]
        public void Tutorial_card__status_tag_colour_should_be_green_if_current_score_equals_possible_score()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                currentScore: 10,
                possibleScore: 10
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.StatusTagColour.Should().Be("nhsuk-tag--green");
        }

        [Test]
        public void Tutorial_card_status_tag_colour_should_be_orange_if_current_score_is_less_than_possible_score()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                currentScore: 7,
                possibleScore: 10
            );

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.StatusTagColour.Should().Be("nhsuk-tag--orange");
        }
    }
}
