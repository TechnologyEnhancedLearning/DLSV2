namespace DigitalLearningSolutions.Data.Tests.DataServices.TutorialContentDataServiceTests
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentDataServiceTests
    {
        [Test]
        public void Get_tutorial_content_should_return_tutorial_content()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeEquivalentTo(new TutorialContent(
                "Navigate documents",
                "Working with documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                13
            ));
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_section_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_course_is_inactive()
        {
            // Given
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_section_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorialContent = tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }
    }
}
