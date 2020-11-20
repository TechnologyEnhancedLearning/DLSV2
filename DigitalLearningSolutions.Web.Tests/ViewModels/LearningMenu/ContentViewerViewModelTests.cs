namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class ContentViewerViewModelTests
    {
        private IConfiguration config;
        private const string BaseUrl = "https://example.com";

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void Content_viewer_should_have_html_url()
        {
            // Given
            var expectedHtmlUrl = "https://www.dls.nhs.uk/CMS/CMSContent/Course508/Section1904/Tutorials/Intro to Social Media/itspplayer.html"
                                  + "?CentreID=101&CustomisationID=24861&CandidateID=254480&Version=2&ProgressID=276837&type=learn"
                                  + $"&TrackURL={BaseUrl}/tracking";

            // When
            var contentViewerViewModel = new ContentViewerViewModel(config);

            // Then
            contentViewerViewModel.HtmlSource.Should().Be(expectedHtmlUrl);
        }

        [Test]
        public void Content_viewer_should_have_scorm_url()
        {
            // Given
            var expectedScormUrl = $"{BaseUrl}/scoplayer/sco?CentreID=101&CustomisationID=37545&CandidateID=254480&Version=1"
                                  + "&tutpath=https://www.dls.nhs.uk/cms/CMSContent/Course589/Section2295/Tutorials/2 Patient Reg PDS/imsmanifest.xml";

            // When
            var contentViewerViewModel = new ContentViewerViewModel(config);

            // Then
            contentViewerViewModel.ScormSource.Should().Be(expectedScormUrl);
        }
    }
}
