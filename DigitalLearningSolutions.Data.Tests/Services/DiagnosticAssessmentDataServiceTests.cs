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

    internal class DiagnosticAssessmentDataServiceTests
    {
        private DiagnosticAssessmentDataService diagnosticAssessmentDataService;
        private DiagnosticAssessmentTestHelper diagnosticAssessmentTestHelper;
        private TutorialContentTestHelper tutorialContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<DiagnosticAssessmentDataService>>();
            diagnosticAssessmentDataService = new DiagnosticAssessmentDataService(connection, logger);
            diagnosticAssessmentTestHelper = new DiagnosticAssessmentTestHelper(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_diagnostic_assessment()
        {
            // Given
            const int customisationId = 5148;
            const int candidateId = 95318;
            const int sectionId = 115;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "Level 2 - Microsoft Excel 2010",
                "Using Formulas",
                "Using formulas",
                4,
                7,
                10,
                "https://www.dls.nhs.uk/tracking/MOST/Excel10Core/Assess/L2_Excel_2010_Diag_4.dcr",
                true,
                "https://www.dls.nhs.uk/tracking/MOST/Excel10Core/Assess/L2_Excel_2010_Post_4.dcr",
                false,
                383,
                116
            );
            expectedDiagnosticAssessment.Tutorials.AddRange(
                new[]
                {
                    new DiagnosticTutorial("Create and edit simple formulas", 383),
                    new DiagnosticTutorial("Understand and enforce simple precedence in formulas", 384),
                    new DiagnosticTutorial("Nest parentheses in formulas", 385),
                    new DiagnosticTutorial("Use relative and absolute cell references", 386),
                    new DiagnosticTutorial("Refer to other worksheets", 387),
                    new DiagnosticTutorial("Link other workbooks", 388)
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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedDiagnosticAssessment = new DiagnosticAssessment(
                "5 Jan Test",
                "New",
                "Working with Microsoft Office applications",
                0,
                0,
                13,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/Diagnostic/01-Diag-Working-with-Microsoft-Office-applications/itspplayer.html",
                true,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/01-PLA-Working-with-Microsoft-Office-applications/itspplayer.html",
                true,
                4340,
                994
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

        [Test]
        public void Get_diagnostic_assessment_should_return_null_if_customisation_id_is_invalid()
        {
            // Given
            const int customisationId = 0;
            const int candidateId = 254480;
            const int sectionId = 994;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(541); // Tutorial with DiagStatus 0
            tutorialIds.Should().Equal(535, 536, 537, 538, 539, 540, 542);
        }


        [Test]
        public void Get_diagnostic_assessment_can_have_tutorials_with_false_status()
        {
            // Given
            const int customisationId = 5694;
            const int candidateId = 1;
            const int sectionId = 103;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().Equal(319, 320, 322, 323, 321, 324); // All have CustomisationTutorials.Status = 0
        }

        [Test]
        public void Get_diagnostic_assessment_should_not_have_archived_tutorials()
        {
            // Given
            const int customisationId = 22416;
            const int candidateId = 118178;
            const int sectionId = 1955;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().Equal(3330, 3331, 3332, 3334, 3333);
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_nextTutorial_if_no_tutorials_in_section()
        {
            // Given
            const int candidateId = 254480;
            const int customisationId = 5694;
            const int sectionId = 104;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_should_return_null_nextSection_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_should_skip_tutorials_not_in_customisation()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210934;
                const int customisationId = 18366;
                const int sectionId = 973;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4255);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4256);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 4257);

                // The next tutorial ID in this section is 4258, but the next tutorial selected in CustomisationTutorials is 4263
                const int nextTutorialId = 4263;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_should_skip_empty_sections()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 974;
            
            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int nextSectionId = 978;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_should_skip_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 11;
                const int customisationId = 15937;
                const int sectionId = 392;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 1534);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 1535);

                const int nextTutorialId = 1583; // Skipping over archived 1536, 1537, 1581

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_can_return_smaller_sectionId()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 17668;
            const int sectionId = 747;

            const int nextSectionId = 746;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_sections_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 15062;
                const int sectionId = 246;

                // The tutorials of what would be the next section, 247;
                tutorialContentTestHelper.ArchiveTutorial(1136);

                const int expectedNextSectionId = 248;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_returns_section_with_only_diagnostic_assessment()
        {
            // Given
            const int candidateId = 74411;
            const int customisationId = 5852;
            const int sectionId = 150;

            const int expectedNextSectionId = 151; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int expectedNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                   // Customisations.IsAssessed = 1 and Sections.PLAssessPath is not null
                                                   // When
            var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

            //Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_diagnostic_assessment_nextSection_skips_assessed_section_with_no_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;

                const int originalNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                       // Customisations.IsAssessed = 1
                tutorialContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);
                const int expectedNextSectionId = 106;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                //Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_smaller_tutorialId_for_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                const int nextTutorialId = 928;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_next_tutorialId_with_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 928);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                // Tutorial: 930  OrderByNumber 36
                const int nextTutorialId = 929;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_assessment_nextTutorial_returns_tutorialId_after_shared_orderByNumber()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 8194;
                const int sectionId = 216;

                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 923);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 924);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 925);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 926);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 927);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 928);
                tutorialContentTestHelper.RemoveCustomisationTutorial(customisationId, 929);

                // All in section 216
                // Tutorial: 927  OrderByNumber 34
                // Tutorial: 928  OrderByNumber 35
                // Tutorial: 929  OrderByNumber 35
                // Tutorial: 930  OrderByNumber 36
                const int nextTutorialId = 930;

                // When
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextTutorialId.Should().Be(nextTutorialId);
            }
        }

        [Test]
        public void Get_diagnostic_content_should_return_diagnostic_content()
        {
            // Given
            const int customisationId = 16588;
            const int sectionId = 172;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

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
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_diagnostic_content_should_return_null_if_section_archived_date_is_not_null()
        {
            // When
            const int customisationId = 14212;
            const int sectionId = 261;
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
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
                var result = diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
                tutorialIds.Should()
                    .Equal(923, 924, 925, 926, 927, 928, 929, 930, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940);
            }
        }

        [Test]
        public void Get_diagnostic_content_should_not_return_archived_tutorials()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;

            // When
            var result = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

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
    }
}
