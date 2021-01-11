namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class DiagnosticContentViewModelTests
    {
        private IConfiguration config;
        private List<int> SelectedTutorials;
        private const string BaseUrl = "https://example.com";
        private const int CustomisationId = 5;
        private const int CentreId = 5;
        private const int SectionId = 5;
        private const int CandidateId = 5;
        private const int ProgressId = 5;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
            SelectedTutorials = new List<int>();
        }

        [Test]
        public void Diagnostic_content_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                SelectedTutorials,
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
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent(
                sectionName: sectionName
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                SelectedTutorials,
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
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent();

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                SelectedTutorials,
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
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent();

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                SelectedTutorials,
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
        public void Diagnostic_content_should_parse_html_url()
        {
            // Given
            const int centreId = 11;
            const int customisationId = 22;
            const int candidateId = 33;
            const int sectionId = 44;
            const int currentVersion = 55;
            const int progressId = 66;
            const int plaPassThreshold = 77;
            const string diagAssessPath = "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.03_Diag.dcr";
            var selectedTutorials = new List<int>(new[] { 1, 2, 3 });
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent(
                diagAssessPath: diagAssessPath,
                plaPassThreshold: plaPassThreshold,
                currentVersion: currentVersion
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                selectedTutorials,
                customisationId,
                centreId,
                sectionId,
                progressId,
                candidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.03_Diag.dcr" +
                "?CentreID=11&CustomisationID=22&CandidateID=33&SectionID=44&Version=55&ProgressID=66" +
                $"&type=diag&TrackURL={BaseUrl}/tracking/tracker&objlist=[1,2,3]&plathresh=77"
            );
        }

        [Test]
        public void Diagnostic_content_should_have_tutorials()
        {
            // Given
            const int centreId = 11;
            const int customisationId = 22;
            const int candidateId = 33;
            const int sectionId = 44;
            const int currentVersion = 55;
            const int progressId = 66;
            const int plaPassThreshold = 77;
            const string diagAssessPath = "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.03_Diag.dcr";
            var selectedTutorials = new List<int>(new[] { 4, 5, 6 });
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent(
                diagAssessPath: diagAssessPath,
                plaPassThreshold: plaPassThreshold,
                currentVersion: currentVersion
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                selectedTutorials,
                customisationId,
                centreId,
                sectionId,
                progressId,
                candidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.03_Diag.dcr" +
                "?CentreID=11&CustomisationID=22&CandidateID=33&SectionID=44&Version=55&ProgressID=66" +
                $"&type=diag&TrackURL={BaseUrl}/tracking/tracker&objlist=[4,5,6]&plathresh=77"
            );
        }

        [Test]
        public void Diagnostic_content_should_parse_scorm_url()
        {
            // Given
            const int centreId = 11;
            const int customisationId = 22;
            const int candidateId = 33;
            const int sectionId = 44;
            const int currentVersion = 55;
            const string diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course38/Diagnostic/03_Digital_Literacy_Diag/imsmanifest.xml";
            var diagnosticContent = DiagnosticAssessmentHelper.CreateDefaultDiagnosticContent(
                diagAssessPath: diagAssessPath,
                currentVersion: currentVersion
            );

            // When
            var diagnosticContentViewModel = new DiagnosticContentViewModel(
                config,
                diagnosticContent,
                SelectedTutorials,
                customisationId,
                centreId,
                sectionId,
                ProgressId,
                candidateId
            );

            // Then
            diagnosticContentViewModel.ContentSource.Should().Be(
                $"{BaseUrl}/scoplayer/sco?CentreID=11&CustomisationID=22&CandidateID=33&SectionID=44&Version=55" +
                "&tutpath=https://www.dls.nhs.uk/CMS/CMSContent/Course38/Diagnostic/03_Digital_Literacy_Diag/imsmanifest.xml&type=diag"
            );
        }
    }
}
