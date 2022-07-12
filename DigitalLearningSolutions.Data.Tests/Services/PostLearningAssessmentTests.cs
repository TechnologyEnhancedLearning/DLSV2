namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal partial class PostLearningAssessmentTests
    {
        private PostLearningAssessmentService postLearningAssessmentService;
        private PostLearningAssessmentTestHelper postLearningAssessmentTestHelper;
        private CourseContentTestHelper courseContentTestHelper;
        private SectionContentTestHelper sectionContentTestHelper;
        private TutorialContentTestHelper tutorialContentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<PostLearningAssessmentService>>();
            postLearningAssessmentService = new PostLearningAssessmentService(connection, logger);
            postLearningAssessmentTestHelper = new PostLearningAssessmentTestHelper(connection);
            courseContentTestHelper = new CourseContentTestHelper(connection);
            sectionContentTestHelper = new SectionContentTestHelper(connection);
            tutorialContentTestHelper = new TutorialContentTestHelper(connection);
        }

        [Test]
        public void Get_post_learning_assessment_should_return_assessment_if_enrolled()
        {
            // Given
            const int customisationId = 6062;
            const int candidateId = 84238;
            const int sectionId = 103;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedPostLearningAssessmentService = new PostLearningAssessment(
                "Level 2 - Microsoft Word 2010",
                null,
                "All Modules and Assessments",
                "Working with documents",
                89,
                1,
                1,
                false,
                false,
                true,
                null,
                0,
                85,
                88,
                90,
                104,
                true,
                true,
                "",
                false
            );
            result.Should().BeEquivalentTo(expectedPostLearningAssessmentService);
        }

        [Test]
        public void Get_post_learning_assessment_should_return_assessment_if_not_enrolled()
        {
            // Given
            const int customisationId = 6062;
            const int candidateId = 4236;
            const int sectionId = 103;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            var expectedPostLearningAssessmentService = new PostLearningAssessment(
                "Level 2 - Microsoft Word 2010",
                null,
                "All Modules and Assessments",
                "Working with documents",
                0,
                0,
                0,
                false,
                false,
                true,
                null,
                0,
                85,
                88,
                90,
                104,
                true,
                true,
                "",
                false
            );
            result.Should().BeEquivalentTo(expectedPostLearningAssessmentService);
        }

        [Test]
        public void Get_post_learning_assessment_should_return_null_if_customisation_id_is_invalid()
        {
            // Given
            const int customisationId = 0;
            const int candidateId = 84238;
            const int sectionId = 103;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_assessment_should_return_null_if_section_id_is_invalid()
        {
            // Given
            const int customisationId = 6062;
            const int candidateId = 84238;
            const int sectionId = 0;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_assessment_should_return_null_if_archived_date_is_not_null()
        {
            // Given
            const int customisationId = 14212;
            const int candidateId = 23031;
            const int sectionId = 261;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_assessment_should_return_null_if_pl_assess_path_is_null()
        {
            // Given
            const int customisationId = 809;
            const int candidateId = 86254;
            const int sectionId = 16;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_assessment_should_return_null_if_customisation_is_not_assessed()
        {
            // Given
            const int customisationId = 5988;
            const int candidateId = 78541;
            const int sectionId = 16;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [TestCase(106456, 7561, 112, 75513)]
        [TestCase(172807, 9917, 112, 133084)]
        [TestCase(160541, 11698, 198, 133094)]
        [TestCase(208119, 4918, 136, 167637)]
        [TestCase(296988, 27676, 3070, 281134)]
        public void Get_post_learning_assessment_should_have_same_scores_as_stored_procedure(
            int candidateId,
            int customisationId,
            int sectionId,
            int progressId
        )
        {
            // Given
            var scoresReturnedFromOldStoredProcedure = postLearningAssessmentTestHelper
                .ScoresFromOldStoredProcedure(progressId, sectionId);

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.PostLearningScore.Should().Be(scoresReturnedFromOldStoredProcedure.MaxScorePL);
            result.PostLearningAttempts.Should().Be(scoresReturnedFromOldStoredProcedure.AttemptsPL);
            result.PostLearningPassed.Should().Be(scoresReturnedFromOldStoredProcedure.PLPassed);
            result.PostLearningLocked.Should().Be(scoresReturnedFromOldStoredProcedure.PLLocked);
        }

        [Test]
        public void Get_post_learning_assessment_for_first_section_in_course_has_next_section()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 382;
            const int expectedNextSectionId = 383;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_post_learning_assessment_for_middle_section_in_course_has_next_section()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 383;
            const int expectedNextSectionId = 384;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_post_learning_assessment_for_last_section_in_course_has_no_next_section()
        {
            // Given
            const int customisationId = 15853;
            const int candidateId = 1;
            const int sectionId = 386;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_should_skip_empty_section()
        {
            // Given
            const int customisationId = 18366;
            const int candidateId = 210934;
            const int sectionId = 974;

            // The next section ID in this Application is 975, but the next section with a tutorial selected in
            // CustomisationTutorials is 978
            const int expectedNextSectionId = 978;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_can_have_smaller_id()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                const int sectionId = 2201;
                const int expectedNextSectionId = 2193;
                postLearningAssessmentTestHelper.EnablePostLearning(customisationId, sectionId);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_returns_section_with_only_diagnostic_assessment()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 5694;
                const int candidateId = 1;
                const int sectionId = 103;
                const int expectedNextSectionId = 104;
                postLearningAssessmentTestHelper.EnablePostLearning(customisationId, sectionId);

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_returns_section_with_only_post_learning_assessment()
        {
            // Given
            const int customisationId = 10820;
            const int candidateId = 1;
            const int sectionId = 104;
            const int expectedNextSectionId = 105;

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.NextSectionId.Should().Be(expectedNextSectionId);
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_skips_assessed_section_with_no_assessment_path()
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
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [TestCase(2087, 2195)]
        [TestCase(2195, 2199)]
        [TestCase(2199, 2086)]
        public void Get_post_learning_assessment_next_section_id_has_correct_ids_when_shared_section_number(
            int sectionId,
            int expectedNextSectionId
        )
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                postLearningAssessmentTestHelper.EnablePostLearning(customisationId, sectionId);
                sectionContentTestHelper.UpdateSectionNumber(2195, 10);
                // Doing this should result in the following sequence:
                // SectionID: 2087, SectionNumber: 6
                // SectionID: 2195, SectionNumber: 10
                // SectionID: 2199, SectionNumber: 10
                // SectionID: 2086, SectionNumber: 11

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_returns_null_when_shared_section_numbers_are_last_in_sequence()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 24057;
                const int candidateId = 1;
                const int sectionId = 2202;
                postLearningAssessmentTestHelper.EnablePostLearning(customisationId, sectionId);
                sectionContentTestHelper.UpdateSectionNumber(2092, 21);

                // Doing this should result in the following sequence:
                // SectionID: 2092, SectionNumber: 21
                // SectionID: 2202, SectionNumber: 21
                // NULL

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.NextSectionId.Should().BeNull();
            }
        }

        [Test]
        public void Get_post_learning_assessment_next_section_id_skips_archived_sections()
        {
            // Given
            const int candidateId = 118178;
            const int customisationId = 22416;
            const int sectionId = 1958;

            const int nextSectionId = 1960; // Skips archived section 1959

            // When
            var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

            // Then
            result.Should().NotBeNull();
            result!.NextSectionId.Should().Be(nextSectionId);
        }

        [Test]
        public void Get_post_learning_assessment_next_section_skips_sections_full_of_archived_tutorials()
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
                postLearningAssessmentTestHelper.EnablePostLearning(customisationId, sectionId);

                const int expectedNextSectionId = 2088;

                // When
                var result = postLearningAssessmentService.GetPostLearningAssessment(customisationId, candidateId, sectionId);

                // Then
                result.Should().NotBeNull();
                result!.NextSectionId.Should().Be(expectedNextSectionId);
            }
        }

        [Test]
        public void Get_post_learning_content_should_return_post_learning_content()
        {
            // Given
            const int customisationId = 24996;
            const int sectionId = 847;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            var expectedPostLearningContent = new PostLearningContent(
                "Outlook 2013 for the Workplace",
                "Full Course",
                "Introducing email",
                "https://www.dls.nhs.uk/CMS/CMSContent/Course257/PLAssess/01-PLA-Introducing-email/itspplayer.html",
                85,
                2
            );
            expectedPostLearningContent.Tutorials.AddRange(new[] { 3545, 3546 });
            result.Should().BeEquivalentTo(expectedPostLearningContent);
        }

        [Test]
        public void Get_post_learning_content_should_not_return_tutorials_where_archived_date_is_not_null()
        {
            // Given
            const int customisationId = 18113;
            const int sectionId = 909;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Tutorials.Should().NotContain(3899);
        }

        [Test]
        public void Get_post_learning_content_should_return_null_if_customisation_id_is_invalid()
        {
            // Given
            const int customisationId = 0;
            const int sectionId = 847;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_content_should_return_null_if_section_id_is_invalid()
        {
            // Given
            const int customisationId = 24996;
            const int sectionId = 0;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_content_should_return_null_if_section_archived_date_is_not_null()
        {
            // Given
            const int customisationId = 18885;
            const int sectionId = 1024;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_content_should_return_null_if_plAssessPath_is_null()
        {
            // Given
            const int customisationId = 21605;
            const int sectionId = 1762;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_post_learning_content_should_return_null_if_isAssessed_is_null()
        {
            // Given
            const int customisationId = 16368;
            const int sectionId = 407;

            // When
            var result = postLearningAssessmentService.GetPostLearningContent(customisationId, sectionId);

            // Then
            result.Should().BeNull();
        }
    }
}
