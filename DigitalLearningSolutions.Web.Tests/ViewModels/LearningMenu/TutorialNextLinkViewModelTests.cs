namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class TutorialNextLinkViewModelTests
    {
        private const int CustomisationId = 1;
        private const int SectionId = 10;
        const string PostLearningAssessmentPath = "/assessment";
        const int NextTutorialId = 101;
        const int NextSectionId = 102;

        [Test]
        public void Tutorial_should_have_customisationId()
        {
            // When
            var nextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                PostLearningAssessmentPath,
                NextTutorialId,
                NextSectionId
            );

            // Then
            nextLinkViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Tutorial_should_have_sectionId()
        {
            // When
            var nextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                PostLearningAssessmentPath,
                NextTutorialId,
                NextSectionId
            );

            // Then
            nextLinkViewModel.SectionId.Should().Be(SectionId);
        }

        [Test]
        public void Tutorial_should_have_postLearningAssessmentPath()
        {
            // When
            var nextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                PostLearningAssessmentPath,
                NextTutorialId,
                NextSectionId
            );

            // Then
            nextLinkViewModel.PostLearningAssessmentPath.Should().Be(PostLearningAssessmentPath);
        }

        [Test]
        public void Tutorial_should_have_nextTutorialId()
        {
            // When
            var nextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                PostLearningAssessmentPath,
                NextTutorialId,
                NextSectionId
            );

            // Then
            nextLinkViewModel.NextTutorialId.Should().Be(NextTutorialId);
        }

        [Test]
        public void Tutorial_should_have_nextSectionId()
        {
            // When
            var nextLinkViewModel = new TutorialNextLinkViewModel(
                CustomisationId,
                SectionId,
                PostLearningAssessmentPath,
                NextTutorialId,
                NextSectionId
            );

            // Then
            nextLinkViewModel.NextSectionId.Should().Be(NextSectionId);
        }
    }
}
