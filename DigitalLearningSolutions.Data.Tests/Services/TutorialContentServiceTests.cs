namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class TutorialContentServiceTests
    {
        private TutorialContentService tutorialContentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentService = new TutorialContentService(connection);
        }

        [Test]
        public void Get_tutorial_information_should_return_tutorial()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(new TutorialInformation(
                50,
                "Navigate documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "Complete",
                3,
                5,
                0,
                3,
                true,
                0,
                "<html><head><title>Tutorial Objective</title></head><body>In this tutorial you will learn to:" +
                "<ul><li>use the Go To feature to jump to a particular page</li><li>browse a document by a specific element</li></ul></body></html>",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs"
            ));
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_section_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_tutorial_with_default_progress_if_candidate_not_on_course()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(new TutorialInformation(
                50,
                "Navigate documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "Not started",
                0,
                5,
                0,
                3,
                true,
                0,
                "<html><head><title>Tutorial Objective</title></head><body>In this tutorial you will learn to:" +
                "<ul><li>use the Go To feature to jump to a particular page</li><li>browse a document by a specific element</li></ul></body></html>",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs"
            ));
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_course_is_inactive()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_tutorial_content()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeEquivalentTo(new TutorialContent(
                "Navigate documents",
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
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

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
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

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
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

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
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

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
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_tutorial_video()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeEquivalentTo(new TutorialVideo(
                "Navigate documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf"
            ));
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_section_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_course_is_inactive()
        {
            // Given
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_videoPath_is_null()
        {
            // Given
            const int customisationId = 4207;
            const int sectionId = 152;
            const int tutorialId = 642;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }
    }
}
