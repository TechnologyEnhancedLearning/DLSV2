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
                "Working with documents",
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
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                51,
                75
            ));
        }

        [Test]
        public void Get_tutorial_information_should_return_null_nextTutorial_if_last_tutorial_in_section()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_nextSection_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;
            const int tutorialId = 94;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_should_skip_tutorials_not_in_customisation()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 973;
            const int tutorialId = 4257;

            // The next tutorial ID in this section is 4258, but the next tutorial selected in CustomisationTutorials is 4263
            const int nextTutorialId = 4263;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_should_skip_empty_sections()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 974;
            const int tutorialId = 4262;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int nextSectionId = 978;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_can_return_smaller_tutorialId()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 17731;
            const int sectionId = 801;
            const int tutorialId = 3334;

            const int nextTutorialId = 3333;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_can_return_smaller_sectionId()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 24057;
            const int sectionId = 2201;
            const int tutorialId = 10184;

            const int nextSectionId = 2193;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_smaller_tutorialId_for_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 927;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            const int nextTutorialId = 928;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_next_tutorialId_with_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 928;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 929;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_tutorialId_after_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 929;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 930;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_should_return_null_postLearningAssessmentPath_when_isAssessed_is_false()
        {
            // Given
            const int candidateId = 11;
            const int customisationId = 24224;
            const int sectionId = 245;
            const int tutorialId = 4407;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.PostLearningAssessmentPath.Should().BeNull();
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
                "Working with documents",
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
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                51,
                75
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
                "Working with documents",
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
