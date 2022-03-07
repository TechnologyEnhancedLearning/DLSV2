namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class PostLearningContentViewModelTests
    {
        private IConfiguration config = null!;

        private const string BaseUrl = "https://example.com";
        private const int CustomisationId = 5;
        private const int CentreId = 6;
        private const int SectionId = 7;
        private const int CandidateId = 8;
        private const int ProgressId = 9;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void Post_learning_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Post_learning_content_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent(
                sectionName: sectionName
            );

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Post_learning_content_should_have_customisation_id()
        {
            // Given
            const int customisationId = 11;
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                customisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Post_learning_content_should_have_section_id()
        {
            // Given
            const int sectionId = 22;
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                CustomisationId,
                CentreId,
                sectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.SectionId.Should().Be(sectionId);
        }

        [Test]
        public void Post_learning_content_should_parse_html_url()
        {
            // Given
            const int currentVersion = 55;
            const int postLearningPassThreshold = 77;
            const string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/03-PLA-Working-with-files/itspplayer.html";
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent(
                postLearningAssessmentPath: diagnosticAssessmentPath,
                postLearningPassThreshold: postLearningPassThreshold,
                currentVersion: currentVersion
            );
            postLearningContent.Tutorials.AddRange(new[] { 1, 2, 3 });

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.ContentSource.Should().Be(
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/03-PLA-Working-with-files/itspplayer.html" +
                "?CentreID=6&CustomisationID=5&CandidateID=8&SectionID=7&Version=55&ProgressID=9" +
                $"&type=pl&TrackURL={BaseUrl}/tracking/tracker&objlist=[1,2,3]&plathresh=77"
            );
        }

        [Test]
        public void Post_learning_content_should_parse_scorm_url()
        {
            // Given
            const int currentVersion = 55;
            const string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course38/PLAssess/04_Digital_Literacy_PL/imsmanifest.xml";
            var postLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent(
                postLearningAssessmentPath: diagnosticAssessmentPath,
                currentVersion: currentVersion
            );

            // When
            var postLearningContentViewModel = new PostLearningContentViewModel(
                config,
                postLearningContent,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            postLearningContentViewModel.ContentSource.Should().Be(
                $"{BaseUrl}/scoplayer/sco?CentreID=6&CustomisationID=5&CandidateID=8&SectionID=7&Version=55" +
                "&tutpath=https://www.dls.nhs.uk/CMS/CMSContent/Course38/PLAssess/04_Digital_Literacy_PL/imsmanifest.xml&type=pl"
            );
        }
    }
}
