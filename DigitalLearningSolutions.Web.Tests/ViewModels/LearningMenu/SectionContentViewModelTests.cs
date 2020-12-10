namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class SectionContentViewModelTests
    {
        [Test]
        public void Section_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Section_content_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                sectionName: sectionName
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Section_content_with_has_learning_should_have_percent_complete()
        {
            // Given
            const bool hasLearning = true;
            const double percentComplete = 0.5;
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be($"{percentComplete:f0}% Complete");
        }

        [Test]
        public void Section_content_without_has_learning_should_have_empty_percent_complete()
        {
            // Given
            const bool hasLearning = false;
            const double percentComplete = 0.5;
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.PercentComplete.Should().Be("");
        }

        [Test]
        public void Section_content_should_have_time_information()
        {
            // Given
            const int timeMins = 4;
            const int averageSectionTime = 10;
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent(
                timeMins: timeMins,
                averageSectionTime: averageSectionTime
            );

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.TimeInformation.Should().Be($"{timeMins}m (average time {averageSectionTime}m)");
        }

        [Test]
        public void Section_content_should_have_customisation_id()
        {
            // Given
            const int customisationId = 5;
            var sectionContent = SectionContentHelper.CreateDefaultSectionContent();

            // When
            var sectionContentViewModel = new SectionContentViewModel(sectionContent, customisationId);

            // Then
            sectionContentViewModel.CustomisationId.Should().Be(customisationId);
        }
    }
}
