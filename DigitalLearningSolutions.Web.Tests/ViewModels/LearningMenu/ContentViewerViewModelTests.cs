namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class ContentViewerViewModelTests
    {
        private IConfiguration config = null!;

        private const string BaseUrl = "https://example.com";
        private const int CustomisationId = 37545;
        private const int CentreId = 101;
        private const int SectionId = 3;
        private const int TutorialId = 4;
        private const int CandidateId = 254480;
        private const int DelegateUserID = 1;
        private const int ProgressId = 276837;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void Content_viewer_should_have_customisationId()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.CustomisationId.Should().Be(CustomisationId);
        }

        [Test]
        public void Content_viewer_should_have_centreId()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.CentreId.Should().Be(CentreId);
        }

        [Test]
        public void Content_viewer_should_have_sectionId()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.SectionId.Should().Be(SectionId);
        }

        [Test]
        public void Content_viewer_should_have_tutorialId()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.TutorialId.Should().Be(TutorialId);
        }

        [Test]
        public void Content_viewer_should_have_progressId()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.ProgressId.Should().Be(ProgressId);
        }

        [Test]
        public void Content_viewer_should_have_tutorialName()
        {
            // Given
            const string tutorialName = "Tutorial Name";
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent(
                tutorialName
            );

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.TutorialName.Should().BeEquivalentTo(tutorialName);
        }

        [Test]
        public void Content_viewer_should_have_sectionName()
        {
            // Given
            const string sectionName = "Section Name";
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent(
                sectionName: sectionName
            );

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.SectionName.Should().BeEquivalentTo(sectionName);
        }

        [Test]
        public void Content_viewer_should_have_courseTitle()
        {
            // Given
            const string applicationName = "Application Name";
            const string customisationName = "Customisation Name";
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            var courseTitle = $"{applicationName} - {customisationName}";
            contentViewerViewModel.CourseTitle.Should().BeEquivalentTo(courseTitle);
        }

        [Test]
        public void Content_viewer_should_parse_scorm_source()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent(
                currentVersion: 1,
                tutorialPath: "https://www.dls.nhs.uk/cms/CMSContent/Course589/Section2295/Tutorials/2 Patient Reg PDS/imsmanifest.xml"
            );

            var expectedScormUrl = $"{BaseUrl}/scoplayer/sco?CentreID=101&CustomisationID=37545&TutorialID=4&CandidateID=254480&Version=1"
                                   + "&tutpath=https://www.dls.nhs.uk/cms/CMSContent/Course589/Section2295/Tutorials/2 Patient Reg PDS/imsmanifest.xml";

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.ContentSource.Should().Be(expectedScormUrl);
        }

        [Test]
        public void Content_viewer_should_parse_html_source()
        {
            // Given
            const int customisationId = 24861;
            var expectedHtmlUrl = "https://www.dls.nhs.uk/CMS/CMSContent/Course508/Section1904/Tutorials/Intro to Social Media/itspplayer.html"
                                + "?CentreID=101&CustomisationID=24861&TutorialID=4&CandidateID=254480&Version=2&ProgressID=276837&type=learn"
                                + $"&TrackURL={BaseUrl}/tracking/tracker";

            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent(
                currentVersion: 2,
                tutorialPath: "https://www.dls.nhs.uk/CMS/CMSContent/Course508/Section1904/Tutorials/Intro to Social Media/itspplayer.html"
            );

            // When
            var contentViewerViewModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                customisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                ProgressId
            );

            // Then
            contentViewerViewModel.ContentSource.Should().Be(expectedHtmlUrl);
        }
    }
}
