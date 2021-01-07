namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using System.Transactions;
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
        private DiagnosticAssessmentTestHelper diagnosticAssessmentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<DiagnosticAssessmentService>>();
            diagnosticAssessmentService = new DiagnosticAssessmentService(connection, logger);
            diagnosticAssessmentTestHelper = new DiagnosticAssessmentTestHelper(connection);
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
        public void Get_diagnostic_assessment_should_return_assessment_if_only_is_assessed_is_true()
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

        [TestCase(70093, 3452, 110, 38227)]
        [TestCase(84238, 6062, 105, 55672)]
        [TestCase(83361, 5263, 161, 98818)]
        [TestCase(173505, 11347, 178, 156377)]
        [TestCase(121301, 5903, 169, 180362)]
        [TestCase(181938, 5263, 153, 198839)]
        [TestCase(269044, 10059, 212, 237170)]
        public void Get_diagnostic_assessment_should_have_same_tutorials_as_stored_procedure(
            int candidateId,
            int customisationId,
            int sectionId,
            int progressId
        )
        {
            using (new TransactionScope())
            {
                // Given
                var tutorialIdsReturnedFromOldStoredProcedure = diagnosticAssessmentTestHelper
                    .TutorialsFromOldStoredProcedure(progressId, sectionId)
                    .Select(tutorial => tutorial.TutorialId);

                // When
                var sectionIdsInCourseContent = diagnosticAssessmentService
                    .GetDiagnosticAssessment(customisationId, candidateId, sectionId)?
                    .Tutorials
                    .Select(section => section.Id);

                // Then
                sectionIdsInCourseContent.Should().Equal(tutorialIdsReturnedFromOldStoredProcedure);
            }
        }

        [TestCase(70093, 3452, 110, 38227)]
        [TestCase(84238, 6062, 105, 55672)]
        [TestCase(83361, 5263, 161, 98818)]
        [TestCase(173505, 11347, 178, 156377)]
        [TestCase(121301, 5903, 169, 180362)]
        [TestCase(181938, 5263, 153, 198839)]
        [TestCase(269044, 10059, 212, 237170)]
        public void Get_diagnostic_assessment_should_have_same_scores_as_stored_procedure(
            int candidateId,
            int customisationId,
            int sectionId,
            int progressId
        )
        {
            // Given
            var scoresReturnedFromOldStoredProcedure = diagnosticAssessmentTestHelper
                .ScoresFromOldStoredProcedure(progressId, sectionId)
                .FirstOrDefault();

            // When
            var sectionInCourseContent = diagnosticAssessmentService
                .GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            sectionInCourseContent.SectionScore.Should().Be(scoresReturnedFromOldStoredProcedure.SecScore);
            sectionInCourseContent.MaxSectionScore.Should().Be(scoresReturnedFromOldStoredProcedure.SecOutOf);
            sectionInCourseContent.DiagnosticAttempts.Should().Be(scoresReturnedFromOldStoredProcedure.DiagAttempts);
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content()
        {
            // Given
            const int customisationId = 5032;
            const int sectionId = 156;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            var expectedDiagnosticContent = new DiagnosticContent(
                "Level 2 - Microsoft PowerPoint 2010",
                "UHL",
                "Working with graphics and multimedia",
                "https://www.dls.nhs.uk/tracking/MOST/PowerPoint10/Assess/L2_PowerPoint_2010_Diag_5.dcr",
                true
            );
            expectedDiagnosticContent.Tutorials.AddRange(
                new[] { 658, 659, 660, 661, 662 }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_content_if_only_is_assessed_is_true()
        {
            // Given
            const int customisationId = 2684;
            const int sectionId = 74;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            var expectedDiagnosticContent = new DiagnosticContent(
                "Level 2 - Microsoft Word 2007",
                "Styles and Working with References",
                "Working with documents",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                true
            );
            expectedDiagnosticContent.Tutorials.AddRange(
                new[] { 49, 50, 51, 52 }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_customisation_id_is_invalid()
        {
            // Given
            const int customisationId = 0;
            const int sectionId = 994;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_section_id_is_invalid()
        {
            // Given
            const int customisationId = 18438;
            const int sectionId = 0;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_archived_date_is_null()
        {
            // When
            const int customisationId = 14212;
            const int sectionId = 261;
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_diag_status_and_status_and_is_assessed_are_false()
        {
            // Given
            const int customisationId = 1530;
            const int sectionId = 74;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }
    }
}
