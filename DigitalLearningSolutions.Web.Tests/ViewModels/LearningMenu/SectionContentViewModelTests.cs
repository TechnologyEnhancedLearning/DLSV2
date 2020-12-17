namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class SectionContentViewModelTests
    {
        private const int CustomisationId = 5;
        private const int SectionId = 5;

        [Test]
        public void Section_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Section_content_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                sectionName: sectionName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Section_content_without_has_learning_should_have_empty_percent_complete()
        {
            // Given
            const bool hasLearning = false;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_content_with_no_complete_tutorials_should_have_zero_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            const int expectedPercentComplete = 0;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_all_complete_tutorials_should_have_one_hundred_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2)
            };
            const int expectedPercentComplete = 100;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_all_started_tutorials_should_have_fifty_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1)
            };
            const int expectedPercentComplete = 50;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_mixed_status_tutorials_that_dont_need_rounding_returns_correct_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 1),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2)
            };
            const int expectedPercentComplete = 50;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{expectedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_with_mixed_status_tutorials_that_need_rounding_returns_correct_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            var tutorials = new[]
            {
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 2),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0),
                SectionTutorialHelper.CreateDefaultSectionTutorial(tutStat: 0)
            };
            const int roundedPercentComplete = 33;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(hasLearning: hasLearning);
            sectionContent.Tutorials.AddRange(tutorials);

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{roundedPercentComplete}% Complete");
        }

        [Test]
        public void Section_content_should_have_customisation_id()
        {
            // Given
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent();

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, CustomisationId, SectionId);

            // Then
            sectionContentViewModel.CustomisationId.Should().Be(CustomisationId);
        }
    }
}
