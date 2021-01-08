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
        public void Get_diagnostic_assessment_should_return_null_if_section_archived_date_is_not_null()
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
        public void Get_diagnostic_assessment_should_return_null_if_section_diagAssessPath_is_null()
        {
            // When
            const int customisationId = 22206;
            const int candidateId = 210962;
            const int sectionId = 1920;
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_all_diagStatus_are_false()
        {
            // Given
            const int customisationId = 4831;
            const int candidateId = 11;
            const int sectionId = 75;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_tutorials_with_false_diagStatus()
        {
            // Given
            const int customisationId = 14579;
            const int candidateId = 102375;
            const int sectionId = 135;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(541); // Tutorial with DiagStatus 0
            tutorialIds.Should().Equal(535, 536, 537, 538, 539, 540, 542);
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_archived_tutorials()
        {
            // Given
            const int customisationId = 22416;
            const int candidateId = 118178;
            const int sectionId = 1955;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(9366); // Archived tutorial
            tutorialIds.Should().Equal(9332, 9333);
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_tutorials_with_diagAssessOutOf_less_than_1()
        {
            // Given
            const int customisationId = 21058;
            const int candidateId = 172968;
            const int sectionId = 189;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(806); // Tutorial with DiagAssessOutOf = 0
            tutorialIds.Should().Equal(804, 805, 807);
        }

        [Test]
        public void Get_diagnostic_assessment_should_use_nonzero_originalTutorialIds()
        {
            // Given
            const int customisationId = 26178;
            const int candidateId = 285914;
            const int sectionId = 2479;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            // Tutorial ID 11376 was not replaced, but the other four have been
            tutorialIds.Should().Equal(11376, 1576, 1577, 1578, 1579);
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_use_tutorials_with_diagStatus_0_in_scores()
        {
            // Given
            const int customisationId = 14579;
            const int candidateId = 102375;
            const int sectionId = 135;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.SectionScore.Should().Be(0);
            result!.MaxSectionScore.Should().Be(42); // Not 44 as uspReturnSectionsForCandCust_V2 returns because
                                                     // it counts tutorial 541 with DiagStatus = 0
            result!.DiagnosticAttempts.Should().Be(0);
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_use_archived_tutorials_in_scores()
        {
            // Given
            const int customisationId = 22416;
            const int candidateId = 118178;
            const int sectionId = 1955;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.SectionScore.Should().Be(0);
            result!.MaxSectionScore.Should().Be(2); // Not 3 as uspReturnSectionsForCandCust_V2 returns because
                                                    // it counts archived tutorial 9366
            result!.DiagnosticAttempts.Should().Be(1);
        }

        [Test]
        public void Get_diagnostic_assessment_should_get_tutorialIds_ordered_by_orderBy()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 17731;
            const int sectionId = 801;

            // Tutorial: 3330  OrderByNumber 1
            // Tutorial: 3331  OrderByNumber 2
            // Tutorial: 3332  OrderByNumber 3
            // Tutorial: 3333  OrderByNumber 5
            // Tutorial: 3334  OrderByNumber 4

            // When
            var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().Equal(3330, 3331, 3332, 3334, 3333);
        }

        [Test]
        public void Get_diagnostic_assessment_should_get_tutorialIds_ordered_by_orderBy_then_tutorialId()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                diagnosticAssessmentTestHelper.UpdateDiagAssessOutOf(928, 1);

                // ...
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                // ...

                // When
                var result = diagnosticAssessmentService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
                tutorialIds.Should()
                    .Equal(923, 924, 925, 926, 927, 928, 929, 930, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940);
            }
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content()
        {
            // Given
            const int customisationId = 16588;
            const int sectionId = 172;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            var expectedDiagnosticContent = new DiagnosticContent(
                "Level 1 - Microsoft Word 2010",
                "Beginner",
                "Proofing and printing",
                "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.03_Diag.dcr",
                true,
                85,
                2
            );
            expectedDiagnosticContent.Tutorials.AddRange(
                new[] { 733, 734, 735, 736 }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_not_return_tutorials_where_diagAssess_is_zero()
        {
            // Given
            const int customisationId = 14416;
            const int sectionId = 214;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            var expectedDiagnosticContent = new DiagnosticContent(
                "Entry Level - Win 7, Office 2010",
                "ESR",
                "Switching on and off",
                "https://www.dls.nhs.uk/tracking/entrylevel/win7/Assess/ELW7_0.03_Diag.dcr",
                true,
                85,
                1
            );
            expectedDiagnosticContent.Tutorials.AddRange(
                new[] { 909, 910, 913, 914, 915, 916 }
            );
            result.Should().BeEquivalentTo(expectedDiagnosticContent);
        }

        [Test]
        public void Get_diagnostic_content_should_not_return_tutorials_where_archived_date_is_null()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;

            // When
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            var expectedDiagnosticContent = new DiagnosticContent(
                "Combined Office Course",
                "Word, Excel, and Outlook",
                "Working with tables",
                "https://www.dls.nhs.uk/tracking/RFM/L1_Word10/Assess/L1_2.06_Diag.dcr",
                true,
                85,
                4
            );
            expectedDiagnosticContent.Tutorials.AddRange(
                new[] { 1141, 1139, 1140 }
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
        public void Get_diagnostic_content_should_return_null_if_archived_date_is_not_null()
        {
            // When
            const int customisationId = 14212;
            const int sectionId = 261;
            var result = diagnosticAssessmentService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }
    }
}
