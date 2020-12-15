namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class SectionContentServiceTests
    {
        private SectionContentService sectionContentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SectionContentService>>();
            sectionContentService = new SectionContentService(connection, logger);
        }

        [Test]
        public void Get_section_content_should_return_section_content()
        {
            // When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Office 2013 Essentials for the Workplace",
                "Erin Test 01",
                "Working with Microsoft Office applications",
                0,
                28,
                true,
                0,
                0,
                13,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/Diagnostic/01-Diag-Working-with-Microsoft-Office-applications/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course120/PLAssess/01-PLA-Working-with-Microsoft-Office-applications/itspplayer.html",
                1,
                1,
                true,
                true
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Introduction to applications", 0, "Not started", 0, 17),
                    new SectionTutorial("Common screen elements", 0, "Not started", 0, 11),
                    new SectionTutorial("Using ribbon tabs", 0, "Not started", 0, 6),
                    new SectionTutorial("Getting help", 0, "Not started", 0, 11)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_null_if_customisation_id_is_invalid()
        {
            //When
            const int customisationId = 0;
            const int candidateId = 1;
            const int sectionId = 382;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_null_if_section_id_is_invalid()
        {
            //When
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 0;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

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
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Excel 2013 for the Workplace",
                "Testing",
                "Entering data",
                4,
                28,
                true,
                0,
                0,
                30,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
                0,
                0,
                true,
                true
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Entering data in the Worksheet", 0, "Not started", 0, 9),
                    new SectionTutorial("Copying, moving and Auto-Filling data", 2, "Complete", 3, 14),
                    new SectionTutorial("Define names for cells and cell ranges", 0, "Not started", 0, 10),
                    new SectionTutorial("Using absolute and relative addresses", 0, "Not started", 0, 12),
                    new SectionTutorial("Insert and delete rows, columns and cells", 0, "Not started", 0, 5),
                    new SectionTutorial("Hide and unhide rows or columns", 2, "Complete", 1, 3),
                    new SectionTutorial("Use pick lists", 0, "Not started", 0, 8),
                    new SectionTutorial("Use comments", 0, "Not started", 0, 21), 
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
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Excel 2013 for the Workplace",
                "Testing",
                "Entering data",
                0,
                28,
                true,
                0,
                0,
                30,
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
                0,
                0,
                true,
                true
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("Entering data in the Worksheet", 0, "Not started", 0, 9),
                    new SectionTutorial("Copying, moving and Auto-Filling data", 0, "Not started", 0, 14),
                    new SectionTutorial("Define names for cells and cell ranges", 0, "Not started", 0, 10),
                    new SectionTutorial("Using absolute and relative addresses", 0, "Not started", 0, 12),
                    new SectionTutorial("Insert and delete rows, columns and cells", 0, "Not started", 0, 5),
                    new SectionTutorial("Hide and unhide rows or columns", 0, "Not started", 0, 3),
                    new SectionTutorial("Use pick lists", 0, "Not started", 0, 8),
                    new SectionTutorial("Use comments", 0, "Not started", 0, 21),
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_null_if_archived_date_is_null()
        {
            // When
            const int customisationId = 14212;
            const int candidateId = 23031;
            const int sectionId = 261;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

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
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_diag_status_is_one()
        {
            // When
            const int customisationId = 5982;
            const int candidateId = 59561;
            const int sectionId = 74;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                "Level 2 - MS Word 2007 DIAGNOSTIC TESTING",
                "Working with documents",
                0,
                28,
                false,
                0,
                0,
                18,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                true,
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("View documents", 0, "Not started", 0, 9),
                    new SectionTutorial("Navigate documents", 0, "Not started", 0, 5),
                    new SectionTutorial("Use document properties", 0, "Not started", 0, 2),
                    new SectionTutorial("Save documents", 0, "Not started", 0, 4)
                }
            );

            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_is_assessed_is_one()
        {
            // When
            const int customisationId = 2684;
            const int candidateId = 196;
            const int sectionId = 74;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                "Styles and Working with References",
                "Working with documents",
                0,
                28,
                false,
                0,
                0,
                18,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                false,
                true
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("View documents", 0, "Not started", 0, 9),
                    new SectionTutorial("Navigate documents", 0, "Not started", 0, 5),
                    new SectionTutorial("Use document properties", 0, "Not started", 0, 2),
                    new SectionTutorial("Save documents", 0, "Not started", 0, 4)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }

        [Test]
        public void Get_section_content_should_return_content_if_only_tutorial_status_is_one()
        {
            // When
            const int customisationId = 1499;
            const int candidateId = 6;
            const int sectionId = 74;
            var result = sectionContentService.GetSectionContent(customisationId, candidateId, sectionId);

            // Then
            var expectedSectionContent = new SectionContent(
                "Level 2 - Microsoft Word 2007",
                "Test",
                "Working with documents",
                3,
                28,
                true,
                0,
                0,
                18,
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Diag_1.dcr",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                0,
                0,
                false,
                false
            );
            expectedSectionContent.Tutorials.AddRange(
                new[]
                {
                    new SectionTutorial("View documents", 0, "Not started", 0, 9),
                    new SectionTutorial("Navigate documents", 0, "Not started", 0, 5),
                    new SectionTutorial("Use document properties", 2, "Complete", 1, 2),
                    new SectionTutorial("Save documents", 2, "Complete", 2, 4)
                }
            );
            result.Should().BeEquivalentTo(expectedSectionContent);
        }
    }
}
