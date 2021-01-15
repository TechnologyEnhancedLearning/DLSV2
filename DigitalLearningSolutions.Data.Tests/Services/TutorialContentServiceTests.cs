namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class TutorialContentServiceTests
    {
        private TutorialContentService tutorialContentService;
        private TutorialContentTestHelper tutorialContentTestHelper;
        private SectionContentTestHelper sectionContentTestHelper;
        private CourseContentTestHelper courseContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            tutorialContentService = new TutorialContentService(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
        }

        [Test]
        public void Get_tutorial_information_should_return_tutorial()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(new TutorialInformation(
                50,
                "Navigate documents",
                "Working with documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "Complete",
                3,
                5,
                0,
                3,
                true,
                0,
                "<html><head><title>Tutorial Objective</title></head><body>In this tutorial you will learn to:" +
                "<ul><li>use the Go To feature to jump to a particular page</li><li>browse a document by a specific element</li></ul></body></html>",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                null,
                false,
                null,
                0,
                85,
                0,
                10,
                51,
                75,
                true,
                true
            ));
        }

        [Test]
        public void Get_tutorial_information_should_return_null_nextTutorial_if_last_tutorial_in_section()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_nextSection_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;
            const int tutorialId = 94;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_should_skip_tutorials_not_in_customisation()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 973;
            const int tutorialId = 4257;

            // The next tutorial ID in this section is 4258, but the next tutorial selected in CustomisationTutorials is 4263
            const int nextTutorialId = 4263;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_should_skip_empty_sections()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 18366;
            const int sectionId = 974;
            const int tutorialId = 4262;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int nextSectionId = 978;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_can_return_smaller_tutorialId()
        {
            // Given
            const int candidateId = 210934;
            const int customisationId = 17731;
            const int sectionId = 801;
            const int tutorialId = 3334;

            const int nextTutorialId = 3333;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_skips_archived_tutorial()
        {
            // Given
            const int candidateId = 11;
            const int customisationId = 15937;
            const int sectionId = 392;
            const int tutorialId = 1535;

            const int nextTutorialId = 1583; // Skipping over archived 1536, 1537, 1581

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_can_return_smaller_sectionId()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 24057;
            const int sectionId = 2201;
            const int tutorialId = 10184;

            const int nextSectionId = 2193;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;
            const int tutorialId = 9349;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_sections_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 210962;
                const int customisationId = 24057;
                const int sectionId = 2201;
                const int tutorialId = 10184;

                // The tutorials of what would be the next section, 2193;
                tutorialContentTestHelper.ArchiveTutorial(10161);
                tutorialContentTestHelper.ArchiveTutorial(10195);

                const int expectedNextSectionId = 2088;

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_returns_section_with_only_diagnostic_assessment()
        {
            using (new TransactionScope())
            {

                // Given
                const int candidateId = 74411;
                const int customisationId = 5852;
                const int sectionId = 150;
                const int tutorialId = 634;

                const int expectedNextSectionId = 151; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1
                // Remove post learning assessment
                tutorialContentTestHelper.UpdatePostLearningAssessmentPath(expectedNextSectionId, null);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_section_with_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 74411;
                const int customisationId = 5852;
                const int sectionId = 148;
                const int tutorialId = 628;

                const int originalNextSectionId = 149; // All tutorials are CustomisationTutorials.Status = 0, though some DiagStatus = 1
                // Remove diagnostic and post learning paths
                tutorialContentTestHelper.UpdateDiagnosticAssessmentPath(originalNextSectionId, null);
                tutorialContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);

                const int expectedNextSectionId = 150;

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextSection_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int tutorialId = 331;

            const int expectedNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                   // Customisations.IsAssessed = 1 and Sections.PLAssessPath is not null
            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_tutorial_information_nextSection_skips_assessed_section_with_no_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;
                const int tutorialId = 331;

                const int originalNextSectionId = 105; // All tutorials are CustomisationTutorials.Status and DiagStatus = 0
                                                       // Customisations.IsAssessed = 1
                tutorialContentTestHelper.UpdatePostLearningAssessmentPath(originalNextSectionId, null);
                const int expectedNextSectionId = 106;

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [TestCase(2087, 10166, 2195)]
        [TestCase(2195, 10168, 2199)]
        [TestCase(2199, 10169, 2086)]
        public void Get_section_content_next_section_id_has_correct_ids_when_shared_section_number(
            int sectionId,
            int tutorialId,
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
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_smaller_tutorialId_for_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 927;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            const int nextTutorialId = 928;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_next_tutorialId_with_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 928;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 929;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_nextTutorial_returns_tutorialId_after_shared_orderByNumber()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 8194;
            const int sectionId = 216;
            const int tutorialId = 929;

            // All in section 216
            // Tutorial: 927  OrderByNumber 34
            // Tutorial: 928  OrderByNumber 35
            // Tutorial: 929  OrderByNumber 35
            // Tutorial: 930  OrderByNumber 36
            const int nextTutorialId = 930;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.NextTutorialId.Should().Be(nextTutorialId);
        }

        [Test]
        public void Get_tutorial_information_should_return_null_postLearningAssessmentPath_when_isAssessed_is_false()
        {
            // Given
            const int candidateId = 11;
            const int customisationId = 24224;
            const int sectionId = 245;
            const int tutorialId = 4407;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.PostLearningAssessmentPath.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_section_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_tutorial_with_default_progress_if_candidate_not_on_course()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeEquivalentTo(new TutorialInformation(
                50,
                "Navigate documents",
                "Working with documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "Not started",
                0,
                5,
                0,
                3,
                true,
                0,
                "<html><head><title>Tutorial Objective</title></head><body>In this tutorial you will learn to:" +
                "<ul><li>use the Go To feature to jump to a particular page</li><li>browse a document by a specific element</li></ul></body></html>",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                "/MOST/Word07Core/support.html?popup=1&item=navigateDocs",
                "https://www.dls.nhs.uk/tracking/MOST/Word07Core/Assess/L2_Word_2007_Post_1.dcr",
                null,
                false,
                null,
                0,
                85,
                0,
                10,
                51,
                75,
                true,
                true
            ));
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_course_is_inactive()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int candidateId = 100;
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int candidateId = 23031;
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_return_null_if_section_is_archived()
        {
            // Given
            const int candidateId = 23031;
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_information_should_parse_course_settings()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 1;
                const int customisationId = 1379;
                const int sectionId = 74;
                const int tutorialId = 50;
                const string courseSettingsText =
                    "{\"lm.sp\":false,\"lm.st\":false,\"lm.sl\":false,\"df.sd\":false,"
                    + "\"df.sm\":false,\"df.ss\":false,\"lm:ce\":\"consolidation/exercise\","
                    + "\"lm:si\":\"supporting/information\"}";
                var expectedCourseSettings = new CourseSettings(courseSettingsText);

                courseContentTestHelper.AddCourseSettings(customisationId, courseSettingsText);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.CourseSettings.Should().BeEquivalentTo(expectedCourseSettings);
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_default_course_settings_when_json_is_null()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;
            var defaultSettings = new CourseSettings(null);

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.CourseSettings.Should().BeEquivalentTo(defaultSettings);
        }

        [Test]
        public void Get_tutorial_information_should_have_other_items_in_section_if_another_tutorial_has_status_1()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_diagnostic()
        {
            // Given
            const int candidateId = 210962;
            const int customisationId = 14961;
            const int sectionId = 350;
            const int tutorialId = 1360;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_post_learning()
        {
            // Given
            const int candidateId = 245614;
            const int customisationId = 24001;
            const int sectionId = 2094;
            const int tutorialId = 9705;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_single_tutorial_just_has_section_consolidation()
        {
            // Given
            const int candidateId = 267014;
            const int customisationId = 21669;
            const int sectionId = 1802;
            const int tutorialId = 8593;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_has_no_other_items_in_section()
        {
            // Given
            const int candidateId = 272596;
            const int customisationId = 23048;
            const int sectionId = 2027;
            const int tutorialId = 9526;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Get_tutorial_information_has_no_other_items_in_section_if_other_tutorials_are_archived()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 14895;
                const int candidateId = 22045;
                const int sectionId = 333;
                const int tutorialId = 1339;

                // The tutorials in this section, which does not have a post learning assessment or consolidation path
                tutorialContentTestHelper.ArchiveTutorial(1340);
                tutorialContentTestHelper.ArchiveTutorial(1341);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_has_other_items_in_section_if_another_tutorial_has_diagnostic()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5468;
                const int candidateId = 94705;
                const int sectionId = 102;
                const int tutorialId = 316;

                // Remove diagnostic from this tutorial
                sectionContentTestHelper.UpdateDiagnosticStatus(tutorialId, customisationId, 0);

                // Make other tutorials in this section inaccessible, but they still have a diagnostic status 1
                tutorialContentTestHelper.UpdateTutorialStatus(317, customisationId, 0);
                tutorialContentTestHelper.UpdateTutorialStatus(318, customisationId, 0);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherItemsInSectionExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_other_sections_in_course_if_last_section_in_course()
        {
            // Given
            const int candidateId = 1;
            const int customisationId = 1379;
            const int sectionId = 82;
            const int tutorialId = 94;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_if_in_middle_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;
            const int tutorialId = 1465;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_if_at_end_of_list()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;
            const int tutorialId = 1485;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [TestCase(2087, 10166)]
        [TestCase(2195, 10168)]
        [TestCase(2199, 10169)]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_shared_section_number(
            int sectionId,
            int tutorialId
        )
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_only_other_section_shares_section_number()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;
                const int tutorialId = 2718;

                sectionContentTestHelper.UpdateSectionNumber(668, 1); // Section 664 also has SectionNumber 1, and is
                                                                      // the only other section on the course

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_only_section_in_application()
        {
            // Given
            const int customisationId = 7967;
            const int candidateId = 11;
            const int sectionId = 210;
            const int tutorialId = 885;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeFalse();
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_only_section_not_archived_in_course()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 21727;
                const int candidateId = 210934;
                const int sectionId = 1806;
                const int tutorialId = 8621;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_is_full_of_archived_tutorials()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 17456;
                const int candidateId = 210934;
                const int sectionId = 664;
                const int tutorialId = 2717;

                // The tutorials of what would be the next section, 668
                // This is the only other section on this course
                tutorialContentTestHelper.ArchiveTutorial(2713);
                tutorialContentTestHelper.ArchiveTutorial(2714);
                tutorialContentTestHelper.ArchiveTutorial(2715);
                tutorialContentTestHelper.ArchiveTutorial(2716);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_other_section_only_has_diagnostic_assessment()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int tutorialId = 322;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeTrue();
            }
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_has_no_diagnostic_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int tutorialId = 322;

                // Make this tutorial viewable
                tutorialContentTestHelper.UpdateTutorialStatus(tutorialId, customisationId, 1);

                // Remove diagnostic assessment paths from other sections
                int[] otherSections = { 104, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    tutorialContentTestHelper.UpdateDiagnosticAssessmentPath(section, null)
                );

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_information_should_have_otherSectionsExist_when_other_sections_only_have_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int tutorialId = 326;

            // When
            var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

            // Then
            tutorial.Should().NotBeNull();
            tutorial!.OtherSectionsExist.Should().BeTrue();
        }

        [Test]
        public void Get_tutorial_information_should_not_have_otherSectionsExist_when_other_section_has_no_post_learning_assessment_path()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 10820;
                const int candidateId = 1;
                const int sectionId = 104;
                const int tutorialId = 326;

                // Remove post learning assessment paths from other sections
                int[] otherSections = { 103, 105, 106, 107, 108, 109, 110, 111 };
                otherSections.ToList().ForEach(section =>
                    tutorialContentTestHelper.UpdatePostLearningAssessmentPath(section, null)
                );

                // When
                var tutorial = tutorialContentService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);

                // Then
                tutorial.Should().NotBeNull();
                tutorial!.OtherSectionsExist.Should().BeFalse();
            }
        }

        [Test]
        public void Get_tutorial_content_should_return_tutorial_content()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeEquivalentTo(new TutorialContent(
                "Navigate documents",
                "Working with documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "/MOST/Word07Core/MOST_Word07_1_1_02.dcr",
                13
            ));
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_section_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_course_is_inactive()
        {
            // Given
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_content_should_return_null_if_section_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorialContent = tutorialContentService.GetTutorialContent(customisationId, sectionId, tutorialId);

            // Then
            tutorialContent.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_tutorial_video()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeEquivalentTo(new TutorialVideo(
                "Navigate documents",
                "Working with documents",
                "Level 2 - Microsoft Word 2007",
                "Testing",
                "/MOST/Word07Core/swf/1_1_02_Navigate_documents.swf"
            ));
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_customisation_id_invalid()
        {
            // Given
            const int customisationId = 1378;
            const int sectionId = 74;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_section_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 75;
            const int tutorialId = 50;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_id_invalid()
        {
            // Given
            const int customisationId = 1379;
            const int sectionId = 74;
            const int tutorialId = 500;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_course_is_inactive()
        {
            // Given
            const int customisationId = 1512;
            const int sectionId = 74;
            const int tutorialId = 52;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_status_0()
        {
            // Given
            const int customisationId = 1530;
            const int sectionId = 74;
            const int tutorialId = 49;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_videoPath_is_null()
        {
            // Given
            const int customisationId = 4207;
            const int sectionId = 152;
            const int tutorialId = 642;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_tutorial_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 249;
            const int tutorialId = 1142;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }

        [Test]
        public void Get_tutorial_video_should_return_null_if_section_is_archived()
        {
            // Given
            const int customisationId = 14212;
            const int sectionId = 261;
            const int tutorialId = 1197;

            // When
            var tutorialVideo = tutorialContentService.GetTutorialVideo(customisationId, sectionId, tutorialId);

            // Then
            tutorialVideo.Should().BeNull();
        }
    }
}
