namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class DiagnosticAssessmentServiceTests
    {
        private DiagnosticAssessmentService diagnosticAssessmentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SectionContentService>>();
            diagnosticAssessmentService = new DiagnosticAssessmentService(connection, logger);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_diagnostic_assessment()
        {
            // Given
            const int customisationId = 9684;
            const int candidateId = 22044;
            const int sectionId = 135;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "Level 2 - Microsoft Outlook 2010",
                "Pre- Reg Pharmacists Diagnostics",
                "Introducing Outlook",
                1,
                41,
                44,
                "https://www.dls.nhs.uk/tracking/MOST/Outlook10Core/Assess/L2_Outlook_2010_Diag_1.dcr",
                false
            );
            expectedDiagnosticAssessment.Tutorials.AddRange(
                new[]
                {
                    new DiagnosticTutorial("Explore the Outlook modules", 535),
                    new DiagnosticTutorial("Use the Navigation Pane", 536),
                    new DiagnosticTutorial("Use the To-Do Bar", 537),
                    new DiagnosticTutorial("Create an email", 538),
                    new DiagnosticTutorial("View and read emails", 539),
                    new DiagnosticTutorial("Respond to emails", 540),
                    new DiagnosticTutorial("Categorise items", 541),
                    new DiagnosticTutorial("Flag an email", 542)
        }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticAssessment);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_diagnostic_assessment_if_not_enrolled()
        {
            // Given
            const int customisationId = 18438;
            const int candidateId = 83744;
            const int sectionId = 993;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "5 Jan Test",
                "New",
                "Working with Microsoft Office applications",
                0,
                0,
                13,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/Diagnostic/01-Diag-Working-with-Microsoft-Office-applications/itspplayer.html",
                true
            );
            expectedDiagnosticAssessment.Tutorials.AddRange(
                new[]
                {
                    new DiagnosticTutorial("Introduction to applications", 4340),
                    new DiagnosticTutorial("Common screen elements", 4341),
                    new DiagnosticTutorial("Using ribbon tabs", 4342),
                    new DiagnosticTutorial("Getting help", 4343)
                }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticAssessment);
        }
    }
}
