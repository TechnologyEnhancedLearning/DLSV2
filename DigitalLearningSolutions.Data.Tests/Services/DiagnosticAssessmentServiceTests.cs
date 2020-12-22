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
            var logger = A.Fake<ILogger<DiagnosticAssessmentService>>();
            diagnosticAssessmentService = new DiagnosticAssessmentService(connection, logger);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_diagnostic_assessment()
        {
            // Given
            const int customisationId = 5148;
            const int candidateId = 95318;
            const int sectionId = 115;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "Level 2 - Microsoft Excel 2010",
                "Using Formulas",
                "Using formulas",
                4,
                7,
                10,
                "https://www.dls.nhs.uk/tracking/MOST/Excel10Core/Assess/L2_Excel_2010_Diag_4.dcr",
                true
            );
            expectedDiagnosticAssessment.Tutorials.AddRange(
                new[]
                {
                    new DiagnosticTutorial("Create and edit simple formulas", 383, true),
                    new DiagnosticTutorial("Understand and enforce simple precedence in formulas", 384, true),
                    new DiagnosticTutorial("Nest parentheses in formulas", 385, true),
                    new DiagnosticTutorial("Use relative and absolute cell references", 386, true),
                    new DiagnosticTutorial("Refer to other worksheets", 387, true),
                    new DiagnosticTutorial("Link other workbooks", 388, true)
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
                    new DiagnosticTutorial("Introduction to applications", 4340, true),
                    new DiagnosticTutorial("Common screen elements", 4341, true),
                    new DiagnosticTutorial("Using ribbon tabs", 4342, true),
                    new DiagnosticTutorial("Getting help", 4343, true)
                }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticAssessment);
        }

        [Test]
        public void Get_diagnostic_assessment_returns_assessment_if_only_is_assessed_is_true()
        {
            // Given
            const int customisationId = 2684;
            const int candidateId = 196;
            const int sectionId = 74;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "Level 2 - Microsoft Word 2007",
                "Styles and Working with References",
                "Working with documents",
                0,
                0,
                18,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                true
            );
            result.Should().BeEquivalentTo(expectedDiagnosticAssessment);
        }

        [Test]
        public void Get_diagnostic_assessment_can_return_no_tutorials()
        {
            // Given
            const int customisationId = 9850;
            const int candidateId = 254480;
            const int sectionId = 170;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Tutorials.Should().BeEmpty();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_customisation_id_is_invalid()
        {
            // Given
            const int customisationId = 0;
            const int candidateId = 254480;
            const int sectionId = 994;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_section_id_is_invalid()
        {
            // Given
            const int customisationId = 18438;
            const int candidateId = 254480;
            const int sectionId = 0;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_archived_date_is_null()
        {
            // When
            const int customisationId = 14212;
            const int candidateId = 23031;
            const int sectionId = 261;
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_diag_status_and_status_and_is_assessed_are_false()
        {
            // Given
            const int customisationId = 1530;
            const int candidateId = 23573;
            const int sectionId = 74;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }
    }
}
