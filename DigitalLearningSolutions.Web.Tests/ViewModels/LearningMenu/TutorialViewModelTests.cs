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
        public void Tutorial_should_have_tutorial_infomation()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation();

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.TutorialInformation.Should().BeEquivalentTo(expectedTutorialInformation);
        }

        [Test]
        public void Tutorial_should_have_customisationId()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation();

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_should_have_sectionId()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation();

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.SectionId.Should().Be(SectionId);
        }

        [Test]
        public void Tutorial_should_summarise_duration()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                averageTutorialDuration: 73,
                timeSpent: 41
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.TimeSummary.Should().Be("41m (average time 1h 13m)");
        }

        [Test]
        public void Tutorial_should_summarise_score()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 9,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.ScoreSummary.Should().Be("(score: 9 out of 10)");
        }

        [Test]
        public void Tutorial_should_decide_to_show_progress()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: true,
                attemptCount: 1
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.CanShowProgress.Should().BeTrue();
        }

        [Test]
        public void Tutorial_should_not_decide_to_show_progress_when_attempt_count_is_0()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: true,
                attemptCount: 0
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.CanShowProgress.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_not_decide_to_show_progress_when_canShowDiagnosticStatus_is_false()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                canShowDiagnosticStatus: false,
                attemptCount: 1
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.CanShowProgress.Should().BeFalse();
        }

        [Test]
        public void Tutorial_should_recommend_a_tutorial_when_score_is_not_max()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 9,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.TutorialRecommendation.Should().Be("recommended");
        }

        [Test]
        public void Tutorial_should_not_recommend_a_tutorial_when_score_is_max()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(
                currentScore: 10,
                possibleScore: 10
            );

            // When
            var tutorialViewModel = new TutorialViewModel(expectedTutorialInformation, CustomisationId, SectionId);

            // Then
            tutorialViewModel.TutorialRecommendation.Should().Be("optional");
        }
    }
}
