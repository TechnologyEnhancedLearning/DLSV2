namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class SectionContentDataServiceTests
    {
        private SectionContentDataService sectionContentDataService;
        private SectionContentTestHelper sectionContentTestHelper;
        private CourseContentTestHelper courseContentTestHelper;
        private TutorialContentTestHelper tutorialContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SectionContentDataService>>();
            sectionContentDataService = new SectionContentDataService(connection, logger);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
        }

        [Test]
        public void Get_section_content_should_return_section_content()
        {
            // When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Office 2013 Essentials for the Workplace",
                null,
                "Erin Test 01",
                "Working with Microsoft Office applications",
                true,
                0,
                0,
                13,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/Diagnostic/01-Diag-Working-with-Microsoft-Office-applications/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/01-PLA-Working-with-Microsoft-Office-applications/itspplayer.html",
                1,
                1,
                true,
                true,
                null,
                null,
                false,
                null,
                0,
                85,
                85,
                100,
                true,
                383,
                "",
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Introduction to applications", 0, "Not started", 0, 17, 1461, true, 0 , 2, true, 0),
                    new SectionTutorial("Common screen elements", 0, "Not started", 0, 11, 1462, true, 0, 4, true, 0),
                    new SectionTutorial("Using ribbon tabs", 0, "Not started", 0, 6, 1463, true, 0, 2, true, 0),
                    new SectionTutorial("Getting help", 0, "Not started", 0, 11, 1464, true, 0, 5, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_null_if_customisation_id_is_invalid()
        {
            // When
            const int customisationId = 0;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_section_id_does_not_exist()
        {
            // When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 1;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_section_is_not_in_this_course()
        {
            // When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 0;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_correct_section_when_percent_complete_is_not_zero()
        {
            // When
            const int customisationId = 19262;
            const int candidateId = 1;
            const int sectionId = 1011;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Excel 2013 for the Workplace",
                null,
                "Testing",
                "Entering data",
                true,
                0,
                0,
                30,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
                0,
                0,
                true,
                true,
                null,
                null,
                true,
                null,
                0,
                85,
                85,
                100,
                true,
                1012,
                "",
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Entering data in the Worksheet", 0, "Not started", 0, 9, 4453, true, 0, 5, true, 0),
                    new SectionTutorial("Copying, moving and Auto-Filling data", 2, "Complete", 3, 14, 4454, true, 0, 5, true, 0),
                    new SectionTutorial("Define names for cells and cell ranges", 0, "Not started", 0, 10, 4455, true, 0, 4, true, 0),
                    new SectionTutorial("Using absolute and relative addresses", 0, "Not started", 0, 12, 4456, true, 0, 3, true, 0),
                    new SectionTutorial("Insert and delete rows, columns and cells", 0, "Not started", 0, 5, 4457, true, 0, 4, true, 0),
                    new SectionTutorial("Hide and unhide rows or columns", 2, "Complete", 1, 3, 4458, true, 0, 2, true, 0),
                    new SectionTutorial("Use pick lists", 0, "Not started", 0, 8, 4459, true, 0, 1, true, 0),
                    new SectionTutorial("Use comments", 0, "Not started", 0, 21, 4460, true, 0, 6, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_still_return_content_if_candidate_is_not_enrolled()
        {
            // Given
            const int customisationId = 19262;
            const int candidateId = 0;
            const int sectionId = 1011;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Excel 2013 for the Workplace",
                null,
                "Testing",
                "Entering data",
                true,
                0,
                0,
                30,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
                0,
                0,
                true,
                true,
                null,
                null,
                true,
                null,
                0,
                85,
                85,
                100,
                true,
                1012,
                "",
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Entering data in the Worksheet", 0, "Not started", 0, 9, 4453, true, 0, 5, true, 0),
                    new SectionTutorial("Copying, moving and Auto-Filling data", 0, "Not started", 0, 14, 4454, true, 0, 5, true, 0),
                    new SectionTutorial("Define names for cells and cell ranges", 0, "Not started", 0, 10, 4455, true, 0, 4, true, 0),
                    new SectionTutorial("Using absolute and relative addresses", 0, "Not started", 0, 12, 4456, true, 0, 3, true, 0),
                    new SectionTutorial("Insert and delete rows, columns and cells", 0, "Not started", 0, 5, 4457, true, 0, 4, true, 0),
                    new SectionTutorial("Hide and unhide rows or columns", 0, "Not started", 0, 3, 4458, true, 0, 2, true, 0),
                    new SectionTutorial("Use pick lists", 0, "Not started", 0, 8, 4459, true, 0, 1, true, 0),
                    new SectionTutorial("Use comments", 0, "Not started", 0, 21, 4460, true, 0, 6, true, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_null_if_archived_date_is_not_null()
        {
            // When
            const int customisationId = 14212;
            const int candidateId = 23031;
            const int sectionId = 261;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_diag_status_and_is_assessed_and_tutorial_status_are_not_equal_to_one()
        {
            // When
            const int customisationId = 1530;
            const int candidateId = 23573;
            const int sectionId = 74;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_diag_status_is_one()
        {
            // When
            const int customisationId = 5994;
            const int candidateId = 6;
            const int sectionId = 74;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                null,
                "Diagnostics Testing",
                "Working with documents",
                false,
                1,
                14,
                18,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                true,
                false,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/cons/WC07-Exercise_1.zip",
                null,
                false,
                DateTime.Parse("2013-07-30 09:10:54.440"),
                0,
                85,
                0,
                100,
                true,
                75,
                "",
                false
            );
            // Will have no tutorials as CustomisationTutorial.Status is 0 for all tutorials in this section

            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_is_assessed_is_one()
        {
            // When
            const int customisationId = 2684;
            const int candidateId = 196;
            const int sectionId = 74;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                null,
                "Styles and Working with References",
                "Working with documents",
                false,
                0,
                0,
                0,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                false,
                true,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/cons/WC07-Exercise_1.zip",
                null,
                false,
                null,
                0,
                85,
                0,
                100,
                true,
                75,
                "",
                false
            );
            // Will have no tutorials as CustomisationTutorial.Status is 0 for all tutorials in this section

            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_tutorial_status_is_one()
        {
            // When
            const int customisationId = 1499;
            const int candidateId = 6;
            const int sectionId = 74;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                null,
                "Test",
                "Working with documents",
                true,
                0,
                0,
                0,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                false,
                false,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/cons/WC07-Exercise_1.zip",
                null,
                false,
                null,
                0,
                85,
                0,
                100,
                true,
                75,
                "",
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("View documents", 0, "Not started", 0, 9, 49, true, 0, 10, false, 0),
                    new SectionTutorial("Navigate documents", 0, "Not started", 0, 5, 50, true, 0, 3, false, 0),
                    new SectionTutorial("Use document properties", 2, "Complete", 1, 2, 51, true, 0, 2, false, 0),
                    new SectionTutorial("Save documents", 2, "Complete", 2, 4, 52, true, 0, 3, false, 0)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_order_by_tutorial_id_when_shared_order_by_number()
        {
            // When
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedTutorialOrder = new[]
                { 923, 924, 925, 926, 927, 928, 929, 930, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940 };

            result.Tutorials.Select(tutorial => tutorial.Id).Should().Equal(expectedTutorialOrder);
        }

        [Test]
        public void Get_section_content_should_sort_using_orderByNumber()
        {
            // When
            const int candidateId = 210934;
            const int customisationId = 17731;
            const int sectionId = 801;

            // All in section 801
            // Tutorial: 3330  OrderByNumber 1
            // Tutorial: 3331  OrderByNumber 2
            // Tutorial: 3332  OrderByNumber 3
            // Tutorial: 3333  OrderByNumber 5
            // Tutorial: 3334  OrderByNumber 4
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedTutorialOrder = new[] { 3330, 3331, 3332, 3334, 3333 };

            result.Tutorials.Select(tutorial => tutorial.Id).Should().Equal(expectedTutorialOrder);
        }

        [Test]
        public void Get_section_content_returns_null_when_there_are_no_customisationTutorials()
        {
            // Given
            const int customisationId = 58;
            const int candidateId = 4;
            const int sectionId = 1;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_returns_null_if_customisation_is_inactive()
        {
            // When
            const int candidateId = 59561;
            const int customisationId = 5982;
            const int sectionId = 74;
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_not_use_archived_tutorials_in_maxScore()
        {
            // Given
            const int customisationId = 22416;
            const int candidateId = 118178;
            const int sectionId = 1955;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.MaxSectionScore.Should().Be(2); // Not 3 as uspReturnSectionsForCandCust_V2 returns because
                                                    // it counts archived tutorial 9366
        }

        [Test]
        public void Get_section_content_should_not_use_archived_tutorials_in_scores()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 3698;
                const int candidateId = 4407;
                const int sectionId = 112;
                const int progressId = 32832;

                const int tutorialToArchive = 372;

                tutorialContentTestHelper.ArchiveTutorial(tutorialToArchive);

                // This will should not change the overall DiagnosticAttempts because the tutorial has now been archived
                tutorialContentTestHelper.UpdateDiagnosticAttempts(tutorialToArchive, progressId, 2);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.SectionScore.Should().Be(7);
                result!.DiagnosticAttempts.Should().Be(1);
            }
        }

        [Test]
        public void Get_section_content_should_not_have_archived_tutorials()
        {
            // Given
            const int customisationId = 22416;
            const int candidateId = 118178;
            const int sectionId = 1955;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(9366); // Archived tutorial
            tutorialIds.Should().Equal(9332, 9333);
        }

        [Test]
        public void Get_section_content_should_not_have_tutorials_with_status_0()
        {
            // Given
            const int customisationId = 7669;
            const int candidateId = 22966;
            const int sectionId = 96;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            var tutorialIds = result!.Tutorials.Select(tutorial => tutorial.Id).ToList();
            tutorialIds.Should().NotContain(267); // Status 0 tutorial
            tutorialIds.Should().NotContain(268); // Status 0 tutorial
            tutorialIds.Should().Equal(261, 262, 263, 264, 265, 266);
        }

        [Test]
        public void Get_section_content_can_have_consolidation_path()
        {
            // Given
            const int customisationId = 1499;
            const int candidateId = 6;
            const int sectionId = 74;
            const string expectedConsolidationPath = "https://www.dls.nhs.uk/tracking/MOST/Word07Core/cons/WC07-Exercise_1.zip";

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.ConsolidationPath.Should().Be(expectedConsolidationPath);
        }

        [Test]
        public void Get_section_content_can_have_no_consolidation_path_but_still_return()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            var expectedSectionContent = new SectionContent(
                "Office 2013 Essentials for the Workplace",
                null,
                "Erin Test 01",
                "Working with Microsoft Office applications",
                true,
                0,
                0,
                13,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/Diagnostic/01-Diag-Working-with-Microsoft-Office-applications/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/01-PLA-Working-with-Microsoft-Office-applications/itspplayer.html",
                1,
                1,
                true,
                true,
                null,
                null,
                false,
                null,
                0,
                85,
                85,
                100,
                true,
                383,
                "",
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Introduction to applications", 0, "Not started", 0, 17, 1461, true, 0, 2, true, 0),
                    new SectionTutorial("Common screen elements", 0, "Not started", 0, 11, 1462, true, 0, 4, true, 0),
                    new SectionTutorial("Using ribbon tabs", 0, "Not started", 0, 6, 1463, true, 0, 2, true, 0),
                    new SectionTutorial("Getting help", 0, "Not started", 0, 11, 1464, true, 0, 5, true, 0)
                }
            );

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_parse_course_settings()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 15853;
                const int candidateId = 1;
                const int sectionId = 382;
                const string courseSettingsText =
                    "{\"lm.sp\":false,\"lm.st\":false,\"lm.sl\":false,\"df.sd\":false,"
                   + "\"df.sm\":false,\"df.ss\":false,\"lm.ce\":\"consolidation/exercise\"}";
                var expectedCourseSettings = new CourseSettings(courseSettingsText);

                courseContentTestHelper.AddCourseSettings(customisationId, courseSettingsText);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.CourseSettings.Should().BeEquivalentTo(expectedCourseSettings);
            }
        }

        [Test]
        public void Get_section_content_should_have_default_course_settings_when_json_is_null()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            var defaultSettings = new CourseSettings(null);

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.CourseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void Get_section_content_next_section_id_should_return_id_if_at_start_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            const int expectedNextSectionId = 383;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_should_return_id_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;
            const int expectedNextSectionId = 384;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_should_return_null_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_section_content_next_section_id_should_skip_empty_section()
        {
            // Given
            const int customisationId = 18366;
            const int candidateId = 210934;
            const int sectionId = 974;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int expectedNextSectionId = 978;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_can_have_smaller_id()
        {
            // Given
            const int customisationId = 24057;
            const int candidateId = 1;
            const int sectionId = 2201;
            const int expectedNextSectionId = 2193;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_returns_section_with_only_diagnostic_assessment()
        {
            // Given
            const int customisationId = 5694;
            const int candidateId = 1;
            const int sectionId = 103;
            const int expectedNextSectionId = 104;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_skips_section_with_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int originalNextSectionId = 104;
                sectionContentTestHelper.UpdateDiagnosticAssessmentPath(originalNextSectionId, null);

                const int expectedNextSectionId = 105;

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_section_content_next_section_id_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int expectedNextSectionId = 105;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_section_content_next_section_id_skips_assessed_section_with_no_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;

                const int originalNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                       // Customisations.IsAssessed = 1
                sectionContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);
                const int expectedNextSectionId = 106;

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [TestCase(2087, 2195)]
        [TestCase(2195, 2199)]
        [TestCase(2199, 2086)]
        public void Get_section_content_next_section_id_has_correct_ids_when_shared_section_number(
            int sectionId,
            int expectedNextSectionId
        )
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);
                // Doing this should result in the following sequence:
                // SectionID: 2087, SectionNumber: 6
                // SectionID: 2195, SectionNumber: 10
                // SectionID: 2199, SectionNumber: 10
                // SectionID: 2086, SectionNumber: 11

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_section_content_next_section_id_returns_null_when_shared_section_numbers_are_last_in_sequence()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                const int sectionId = 2202;
                sectionContentTestHelper.UpdateSectionNumber(2092, 21);

                // Doing this should result in the following sequence:
                // SectionID: 2092, SectionNumber: 21
                // SectionID: 2202, SectionNumber: 21
                // NULL

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().BeNull();
            }
        }

        [Test]
        public void Get_section_content_next_section_id_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_section_content_nextSection_skips_sections_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 24057;
                const int sectionId = 2201;

                // The tutorials of what would be the next section, 2193;
                tutorialContentTestHelper.ArchiveTutorial(10161);
                tutorialContentTestHelper.ArchiveTutorial(10195);

                const int expectedNextSectionId = 2088;

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_section_content_ignores_tutorials_with_diag_status_0_when_calculating_max_score()
        {
            // Given
            const int customisationId = 23271;
            const int candidateId = 1128;
            const int sectionId = 137;
            // Adding all scores is 49, adding only ones with diagStatus=1 is 41
            const int expectedMaxScore = 41;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.MaxSectionScore.Should().Be(expectedMaxScore);
        }

        [Test]
        public void Get_section_content_ignores_tutorials_with_diag_status_0_when_calculating_candidate_score()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 23271;
                const int candidateId = 1128;
                const int sectionId = 137;
                const int progressId = 232814;
                const int expectedSectionScore = 0;
                // Tutorial 561 has DiagStatus=0 so it shouldn't be counted meaning the section score is 0 not 1
                tutorialContentTestHelper.UpdateDiagnosticScore(561, progressId, 1);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.SectionScore.Should().Be(expectedSectionScore);
            }
        }

        [Test]
        public void Get_section_content_ignores_tutorials_with_diag_status_0_when_calculating_candidate_attempts()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 23271;
                const int candidateId = 1128;
                const int sectionId = 137;
                const int progressId = 232814;
                const int expectedDiagnosticAttempts = 0;
                // Tutorial 561 has DiagStatus=0 so it shouldn't be counted meaning the diagnostic attempts is 0 not 1
                tutorialContentTestHelper.UpdateDiagnosticAttempts(561, progressId, 1);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.DiagnosticAttempts.Should().Be(expectedDiagnosticAttempts);
            }
        }

        [Test]
        public void Get_section_content_ignores_first_tutorial_when_calculating_max_score_if_tutorial_diag_status_is_0()
        {
            // Given
            const int customisationId = 12036;
            const int candidateId = 162301;
            const int sectionId = 214;
            // Adding all scores is 8 but first tutorial has diagStatus=0 so score should be 7
            const int expectedMaxScore = 7;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.MaxSectionScore.Should().Be(expectedMaxScore);
        }

        [Test]
        public void Get_section_content_ignores_first_tutorial_when_calculating_candidate_score_if_tutorial_diag_status_is_0()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 12036;
                const int candidateId = 162301;
                const int sectionId = 214;
                const int progressId = 129600;
                const int expectedSectionScore = 6;

                // Tutorial 910 has DiagStatus=0 so the section score should be 6 and not 7
                tutorialContentTestHelper.UpdateDiagnosticScore(910, progressId, 1);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.SectionScore.Should().Be(expectedSectionScore);
            }
        }

        [Test]
        public void Get_section_content_ignores_first_tutorial_when_calculating_candidate_attempts_if_tutorial_diag_status_is_0()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 12036;
                const int candidateId = 162301;
                const int sectionId = 214;
                const int progressId = 129600;
                const int expectedDiagAttempts = 2;

                // Tutorial 910 has DiagStatus=0 so attempts should be 2 and not 4
                tutorialContentTestHelper.UpdateDiagnosticAttempts(910, progressId, 4);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.DiagnosticAttempts.Should().Be(expectedDiagAttempts);
            }
        }

        [Test]
        public void Get_section_content_diagnostic_status_is_true_if_only_first_tutorial_has_diag_status_0()
        {
            // Given
            const int customisationId = 12036;
            const int candidateId = 162301;
            const int sectionId = 214;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.DiagnosticStatus.Should().BeTrue();
        }

        [Test]
        public void Get_section_content_diagnostic_status_is_true_if_at_least_one_tutorial_has_diag_status_1()
        {
            // Given
            const int customisationId = 23271;
            const int candidateId = 1128;
            const int sectionId = 137;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.DiagnosticStatus.Should().BeTrue();
        }

        [Test]
        public void Get_section_content_diagnostic_status_is_false_if_all_tutorials_have_diag_status_0()
        {
            // Given
            const int customisationId = 2684;
            const int candidateId = 196;
            const int sectionId = 74;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.DiagnosticStatus.Should().BeFalse();
        }

        [Test]
        public void Get_section_content_diagnostic_status_is_true_if_one_tutorial_has_diag_status_1()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 2684;
                const int candidateId = 196;
                const int sectionId = 74;
                // Set the second tutorial to have DiagStatus=1
                tutorialContentTestHelper.UpdateDiagnosticStatus(50, customisationId, 1);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.DiagnosticStatus.Should().BeTrue();
            }
        }

        [Test]
        public void Get_section_content_should_have_otherSectionsExist_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_section_content_should_have_otherSectionsExist_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.OtherSectionsExist.Should().BeTrue();
        }

        [TestCase(2087)]
        [TestCase(2195)]
        [TestCase(2199)]
        public void Get_section_content_should_have_otherSectionsExist_when_shared_section_number(int sectionId)
        {
            using (new TransactionScope())
            {

                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_section_content_should_have_otherSectionsExist_when_only_other_section_shares_section_number()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;

                sectionContentTestHelper.UpdateSectionNumber(668, 1); // Section 664 also has SectionNumber 1, and is
                // the only other section on the course

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_section_content_should_not_have_otherSectionsExist_when_only_section_in_application()
        {
            // Given
            const int customisationId = 7967;
            const int candidateId = 11;
            const int sectionId = 210;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_section_content_should_not_have_otherSectionsExist_when_only_section_not_archived_in_course()
        {
            // Given
            const int customisationId = 21727;
            const int candidateId = 210934;
            const int sectionId = 1806;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_section_content_should_not_have_otherSectionsExist_when_other_section_is_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;

                // The tutorials of what would be the next section, 668
                // This is the only other section on this course
                tutorialContentTestHelper.ArchiveTutorial(2713);
                tutorialContentTestHelper.ArchiveTutorial(2714);
                tutorialContentTestHelper.ArchiveTutorial(2715);
                tutorialContentTestHelper.ArchiveTutorial(2716);

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_section_content_should_have_otherSectionsExist_when_other_section_only_has_diagnostic_assessment()
        {
            // Given
            const int customisationId = 5694;
            const int candidateId = 1;
            const int sectionId = 103;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_section_content_should_not_have_otherSectionsExist_when_other_section_has_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;

                // Remove diagnostic assessment paths from other sections
                int[] otherSections = { 104, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    sectionContentTestHelper.UpdateDiagnosticAssessmentPath(section, null)
                );

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_section_content_should_have_otherSectionsExist_when_other_sections_only_have_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_section_content_should_not_have_otherSectionsExist_when_other_section_has_no_post_learning_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;

                // Remove post learning assessment paths from other sections
                int[] otherSections = { 103, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    sectionContentTestHelper.UpdatePostLearningAssessmentPath(section, null)
                );

                // When
                var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_section_content_tutorials_should_have_current_score()
        {
            // Given
            const int customisationId = 6698;
            const int candidateId = 22966;
            const int sectionId = 74;
            int[] expectedCurrentScores = { 3, 1, 0, 0 };

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Tutorials.Select(tutorial => tutorial.CurrentScore).Should().Equal(expectedCurrentScores);
        }

        [Test]
        public void Get_section_content_tutorials_should_have_possible_score()
        {
            // Given
            const int customisationId = 6698;
            const int candidateId = 22966;
            const int sectionId = 74;
            int[] expectedPossibleScores = { 10, 3, 2, 3 };

            // When
            var result = sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Tutorials.Select(tutorial => tutorial.PossibleScore).Should().Equal(expectedPossibleScores);
        }

        [Test]
        public void GetSectionsByApplication_should_have_correct_results()
        {
            // When
            var result = sectionContentDataService.GetSectionsForApplication(1).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(10);
                result.First().SectionId.Should().Be(1);
                result.First().SectionName.Should().Be("Mouse skills");
                result.First().Tutorials.Should().BeEmpty();
            }
        }

        [Test]
        public void GetSectionById_returns_expected_section()
        {
            // Given
            const int sectionId = 1;
            const string expectedName = "Mouse skills";

            // When
            var result = sectionContentDataService.GetSectionById(sectionId);

            // Then
            result?.SectionId.Should().Be(sectionId);
            result?.SectionName.Should().Be(expectedName);
        }
    }
}
