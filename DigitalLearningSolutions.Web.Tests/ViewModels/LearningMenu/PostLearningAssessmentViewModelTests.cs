namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class PostLearningAssessmentViewModelTests
    {
        private const int CustomisationId = 5;
        private const int SectionId = 5;

        [Test]
        public void Post_learning_assessment_should_have_title()
        {
            // Given
            const string applicationName = "Application name";
            const string customisationName = "Customisation name";
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.CourseTitle.Should().Be($"{applicationName} - {customisationName}");
        }

        [Test]
        public void Post_learning_assessment_should_have_section_name()
        {
            // Given
            const string sectionName = "Section name";
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                sectionName: sectionName
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.SectionName.Should().Be(sectionName);
        }

        [Test]
        public void Post_learning_assessment_can_be_locked()
        {
            // Given
            const bool postLearningLocked = true;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                plLocked: postLearningLocked
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.PostLearningLocked.Should().BeTrue();
        }

        [Test]
        public void Post_learning_assessment_can_be_not_locked()
        {
            // Given
            const bool postLearningLocked = false;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                plLocked: postLearningLocked
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.PostLearningLocked.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_no_attempts_should_be_not_attempted()
        {
            // Given
            const int postLearningAttempts = 0;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.AssessmentStatus.Should().Be("Not attempted");
        }

        [Test]
        public void Post_learning_assessment_attempts_information_with_attempts_can_be_passed()
        {
            // Given
            const int postLearningAttempts = 4;
            const int postLearningPasses = 2;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                plPasses: postLearningPasses,
                bestScore: 90
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.AssessmentStatus.Should().Be("Passed (90% - 4 attempts)");
        }

        [Test]
        public void Post_learning_assessment_attempts_information_with_attempts_can_be_failed()
        {
            // Given
            const int postLearningAttempts = 5;
            const int postLearningPasses = 0;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                plPasses: postLearningPasses,
                bestScore: 10
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.AssessmentStatus.Should().Be("Failed (10% - 5 attempts)");
        }

        [Test]
        public void Post_learning_assessment_should_have_customisation_id()
        {
            // Given
            const int customisationId = 11;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, customisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Post_learning_assessment_should_have_section_id()
        {
            // Given
            const int sectionId = 22;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, sectionId);

            // Then
            postLearningAssessmentViewModel.SectionId.Should().Be(sectionId);
        }

        [Test]
        public void Post_learning_assessment_can_have_next_section()
        {
            // Given
            const int nextSectionId = 200;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                nextSectionId: nextSectionId
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.NextSectionId.Should().Be(200);
        }

        [Test]
        public void Post_learning_assessment_can_have_no_next_section()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                nextSectionId: null
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.NextSectionId.Should().BeNull();
        }
    }
}
