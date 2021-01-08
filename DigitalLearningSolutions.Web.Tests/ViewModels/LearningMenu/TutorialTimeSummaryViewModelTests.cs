namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class TutorialTimeSummaryViewModelTests
    {
        private const bool ShowTime = true;
        private const bool ShowLearnStatus = true;

        [Test]
        public void Tutorial_card_average_time_spent_string_should_have_plural_if_value_is_not_one()
        {
            // Given
            const int timeSpent = 1;
            const int averageTime = 10;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.AverageTimeSummary.Should().Be($"(average tutorial time {averageTime} minutes)");
        }

        [Test]
        public void Tutorial_card_average_time_spent_string_should_not_have_plural_if_value_is_one()
        {
            // Given
            const int timeSpent = 10;
            const int averageTime = 1;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.AverageTimeSummary.Should().Be($"(average tutorial time {averageTime} minute)");
        }

        [Test]
        public void Tutorial_card_time_spent_string_should_have_plural_if_value_is_not_one()
        {
            // Given
            const int timeSpent = 10;
            const int averageTime = 1;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.TimeSpentSummary.Should().Be($"{timeSpent} minutes spent");
        }

        [Test]
        public void Tutorial_card_time_spent_string_should_not_have_plural_if_value_is_one()
        {
            // Given
            const int timeSpent = 1;
            const int averageTime = 10;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.TimeSpentSummary.Should().Be($"{timeSpent} minute spent");
        }

        [Test]
        public void Tutorial_card_should_show_time_if_courseSettings_are_true()
        {
            // Given
            const int timeSpent = 10;
            const int averageTime = 10;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                showTimeSetting: true,
                showLearnStatusSetting: true
            );

            tutorialTimeSummaryViewModel.ShowTime.Should().BeTrue();
        }

        [Test]
        public void Tutorial_card_should_not_show_time_if_showTime_courseSetting_is_false()
        {
            // Given
            const int timeSpent = 10;
            const int averageTime = 10;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                showTimeSetting: false,
                showLearnStatusSetting: true
            );

            // Then
            tutorialTimeSummaryViewModel.ShowTime.Should().BeFalse();
        }

        [Test]
        public void Tutorial_card_should_not_show_time_if_showLearnStatus_is_false()
        {
            // Given
            const int timeSpent = 10;
            const int averageTime = 10;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                showTimeSetting: true,
                showLearnStatusSetting: false
            );

            // Then
            tutorialTimeSummaryViewModel.ShowTime.Should().BeFalse();
        }
    }
}
