﻿namespace DigitalLearningSolutions.Data.Tests.Services.TutorialContentServiceTests
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using FluentAssertions;
    using NUnit.Framework;

    internal partial class TutorialContentServiceTests
    {
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

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_section_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }
    }
}
