namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class PostLearningAssessmentTests
    {
        private PostLearningAssessmentService postLearningAssessmentService;
        private PostLearningAssessmentTestHelper postLearningAssessmentTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<PostLearningAssessmentService>>();
            postLearningAssessmentService = new PostLearningAssessmentService(connection, logger);
            postLearningAssessmentTestHelper = new PostLearningAssessmentTestHelper(connection);
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
                "All Modules and Assessments",
                "Working with documents",
                89,
                1,
                1,
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
                "All Modules and Assessments",
                "Working with documents",
                0,
                0,
                0,
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
        public void Get_post_learning_content_should_return_null_if_archived_date_is_not_null()
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
