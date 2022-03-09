namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class DiagnosticContentViewModelTests
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
        public void Diagnostic_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var emptySelectedTutorials = new List<int>();
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                emptySelectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Diagnostic_content_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var emptySelectedTutorials = new List<int>();
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(
                sectionName: sectionName
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                emptySelectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Diagnostic_content_should_have_customisation_id()
        {
            // Given
            const int customisationId = 11;
            var emptySelectedTutorials = new List<int>();
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                emptySelectedTutorials,
                customisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Diagnostic_content_should_have_section_id()
        {
            // Given
            const int sectionId = 22;
            var emptySelectedTutorials = new List<int>();
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                emptySelectedTutorials,
                CustomisationId,
                CentreId,
                sectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.SectionId.Should().Be(sectionId);
        }

        [Test]
        public void Diagnostic_content_should_parse_html_url_using_selected_tutorials_when_can_select_tutorials_is_true()
        {
            // Given
            const int currentVersion = 55;
            const int postLearningPassThreshold = 77;
            const string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DiagnosticTesting/itspplayer.html";
            var selectedTutorials = new List<int>(new[] { 1, 2, 3 });
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(
                diagnosticAssessmentPath: diagnosticAssessmentPath,
                postLearningPassThreshold: postLearningPassThreshold,
                currentVersion: currentVersion,
                canSelectTutorials: true
            );
            diagnosticContent.Tutorials.AddRange(new[] { 1, 2, 3, 4 });

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                selectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DiagnosticTesting/itspplayer.html" +
                "?CentreID=6&CustomisationID=5&CandidateID=8&SectionID=7&Version=55&ProgressID=9" +
                $"&type=diag&TrackURL={BaseUrl}/tracking/tracker&objlist=[1,2,3]&plathresh=77"
            );
        }

        [Test]
        public void Diagnostic_content_should_parse_html_url_using_all_tutorials_when_can_select_tutorials_is_false()
        {
            // Given
            const int currentVersion = 55;
            const int postLearningPassThreshold = 77;
            const string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DiagnosticTesting/itspplayer.html";
            var selectedTutorials = new List<int>(new[] { 1, 2, 3 });
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(
                diagnosticAssessmentPath: diagnosticAssessmentPath,
                postLearningPassThreshold: postLearningPassThreshold,
                currentVersion: currentVersion,
                canSelectTutorials: false
            );
            diagnosticContent.Tutorials.AddRange(new[] { 1, 2, 3, 4 });

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                selectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                "https://www.dls.nhs.uk/CMS/CMSContent/Course119/Diagnostic/07DiagnosticTesting/itspplayer.html" +
                "?CentreID=6&CustomisationID=5&CandidateID=8&SectionID=7&Version=55&ProgressID=9" +
                $"&type=diag&TrackURL={BaseUrl}/tracking/tracker&objlist=[1,2,3,4]&plathresh=77"
            );
        }

        [Test]
        public void Diagnostic_content_should_parse_scorm_url()
        {
            // Given
            const int currentVersion = 55;
            var emptySelectedTutorials = new List<int>();
            const string diagnosticAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course38/Diagnostic/03_Digital_Literacy_Diag/imsmanifest.xml";
            var diagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent(
                diagnosticAssessmentPath: diagnosticAssessmentPath,
                currentVersion: currentVersion
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                emptySelectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                ProgressId,
                CandidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                $"{BaseUrl}/scoplayer/sco?CentreID=6&CustomisationID=5&CandidateID=8&SectionID=7&Version=55" +
                "&tutpath=https://www.dls.nhs.uk/CMS/CMSContent/Course38/Diagnostic/03_Digital_Literacy_Diag/imsmanifest.xml&type=diag"
            );
        }
    }
}
