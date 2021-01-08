namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class TutorialTimeSummaryViewModelTests
    {
        private const bool ShowTime = true;
        private const bool ShowLearnStatus = true;

        [TestCase(0, "0 minutes")]
        [TestCase(1, "1 minute")]
        [TestCase(30, "30 minutes")]
        [TestCase(120, "2 hours")]
        [TestCase(61, "1 hour 1 minute")]
        [TestCase(195, "3 hours 15 minutes")]
        public void TutorialTimeSummary_formats_average_time_spent(int averageTime, string expectedFormattedTime)
        {
            // Given
            const int timeSpent = 1;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.AverageTimeSummary.Should().Be(expectedFormattedTime);
        }

        [TestCase(0, "0 minutes")]
        [TestCase(1, "1 minute")]
        [TestCase(30, "30 minutes")]
        [TestCase(120, "2 hours")]
        [TestCase(61, "1 hour 1 minute")]
        [TestCase(195, "3 hours 15 minutes")]
        public void TutorialTimeSummary_formats_user_time_spent(int timeSpent, string expectedFormattedTime)
        {
            // Given
            const int averageTime = 1;

            // When
            var tutorialTimeSummaryViewModel = new TutorialTimeSummaryViewModel(
                timeSpent,
                averageTime,
                ShowTime,
                ShowLearnStatus
            );

            // Then
            tutorialTimeSummaryViewModel.TimeSpentSummary.Should().Be(expectedFormattedTime);
        }

        [Test]
        public void TutorialTimeSummary_should_show_time_if_courseSettings_are_true()
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
        public void TutorialTimeSummary_should_not_show_time_if_showTime_courseSetting_is_false()
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
        public void TutorialTimeSummary_should_not_show_time_if_showLearnStatus_is_false()
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
