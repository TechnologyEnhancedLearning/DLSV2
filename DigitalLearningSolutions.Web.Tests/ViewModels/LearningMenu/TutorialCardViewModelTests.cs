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
        public void Tutorial_card_average_time_spent_string_should_have_plural_if_value_is_not_one()
        {
            // Given
            const int averageTime = 10;
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial(
                averageTutMins: averageTime
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
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

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
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

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
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                ShowTime,
                ShowLearnStatus,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.TimeSpentInformation.Should().Be($"{tutTime} minute spent");
        }

        [Test]
        public void Tutorial_card_should_show_time_if_courseSettings_are_true()
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
            tutorialCardViewModel.ShowTime.Should().BeTrue();
        }

        [Test]
        public void Tutorial_card_should_not_show_time_if_showTime_courseSetting_is_false()
        {
            // Given
            var sectionTutorial = SectionTutorialHelper.CreateDefaultSectionTutorial();

            // When
            var tutorialCardViewModel = new TutorialCardViewModel(
                sectionTutorial,
                showTime: false,
                showLearnStatus: true,
                CustomisationId,
                SectionId
            );

            // Then
            tutorialCardViewModel.ShowTime.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_should_not_show_time_if_showLearnStatus_is_false()
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
            tutorialCardViewModel.ShowTime.Should().BeFalse();
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
