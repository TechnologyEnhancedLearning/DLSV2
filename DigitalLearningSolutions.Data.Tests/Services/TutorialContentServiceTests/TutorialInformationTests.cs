namespace DigitalLearningSolutions.Data.Tests.Services.TutorialContentServiceTests
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentServiceTests
    {
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
                null,
                false,
                true,
                null,
                0,
                85,
                0,
                10,
                51,
                75,
                true,
                true
            ));
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
                null,
                false,
                true,
                null,
                0,
                85,
                0,
                10,
                51,
                75,
                true,
                true
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
        public void Get_tutorial_information_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int candidateId = 23031;
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_section_is_archived()
        {
            // Given
            const int candidateId = 23031;
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_parse_course_settings()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 1379;
                const int sectionId = 74;
                const int tutorialId = 50;
                const string courseSettingsText =
                    "{\"lm.sp\":false,\"lm.st\":false,\"lm.sl\":false,\"df.sd\":false,"
                    + "\"df.sm\":false,\"df.ss\":false,\"lm:ce\":\"consolidation/exercise\","
                    + "\"lm:si\":\"supporting/information\"}";
                var expectedCourseSettings = new CourseSettings(courseSettingsText);

                courseContentTestHelper.AddCourseSettings(customisationId, courseSettingsText);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.CourseSettings.Should().BeEquivalentTo(expectedCourseSettings);
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_default_course_settings_when_json_is_null()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;
            var defaultSettings = new CourseSettings(null);

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.CourseSettings.Should().BeEquivalentTo(defaultSettings);
        }
    }
}
