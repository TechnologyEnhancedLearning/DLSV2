namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using System;

    class TutorialVideoViewModelTests
    {
        private IConfiguration config;
        private const string BaseUrl = "https://example.com";
        private const int CustomisationId = 2;
        private const int SectionId = 3;
        private const int TutorialId = 4;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void TutorialVideo_should_have_customisationId()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void TutorialVideo_should_have_sectionId()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.SectionId.Should().Be(SectionId);
        }

        [Test]
        public void TutorialVideo_should_have_tutorialId()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.TutorialId.Should().Be(TutorialId);
        }

        [Test]
        public void TutorialVideo_should_have_tutorialName()
        {
            // Given
            const string tutorialName = "Tutorial Name";
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo(
                tutorialName
            );

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.TutorialName.Should().BeEquivalentTo(tutorialName);
        }

        [Test]
        public void TutorialVideo_should_have_sectionName()
        {
            // Given
            const string sectionName = "Section Name";
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo(
                sectionName: sectionName
            );

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.SectionName.Should().BeEquivalentTo(sectionName);
        }

        [Test]
        public void TutorialVideo_should_have_courseTitle()
        {
            // Given
            const string applicationName = "Application Name";
            const string customisationName = "Customisation Name";
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            var courseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;
            tutorialVideoViewModel.CourseTitle.Should().BeEquivalentTo(courseTitle);
        }

        [TestCase("https://example.com/testVideo.mp4", "https://example.com/testVideo.mp4")]
        [TestCase("example.com/testVideo.mp4", "https://example.com/testVideo.mp4")]
        [TestCase("/testVideo.mp4", BaseUrl + "/testVideo.mp4")]
        public void TutorialVideo_should_parse_path(
            string givenVideoPath,
            string expectedParsedPath
        )
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo(
                videoPath: givenVideoPath
            );

            // When
            var tutorialVideoViewModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            // Then
            tutorialVideoViewModel.VideoPath.Should().Be(expectedParsedPath);
        }
    }
}
