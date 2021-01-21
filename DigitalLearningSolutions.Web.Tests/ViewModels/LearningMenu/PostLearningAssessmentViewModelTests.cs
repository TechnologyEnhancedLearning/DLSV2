namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
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
        public void Post_learning_assessment_assessment_status_with_attempts_can_be_passed()
        {
            // Given
            const int postLearningAttempts = 4;
            const int postLearningPasses = 2;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                plPasses: postLearningPasses
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.AssessmentStatus.Should().Be("Passed");
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_attempts_can_be_failed()
        {
            // Given
            const int postLearningAttempts = 5;
            const int postLearningPasses = 0;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                plPasses: postLearningPasses
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.AssessmentStatus.Should().Be("Failed");
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_no_attempts_should_have_no_score_information()
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
            postLearningAssessmentViewModel.ScoreInformation.Should().BeNull();
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_one_attempt_should_have_score_information()
        {
            // Given
            const int postLearningAttempts = 1;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                bestScore: 10
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ScoreInformation.Should().Be("(10% - 1 attempt)");
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_multiple_attempts_should_have_score_information()
        {
            // Given
            const int postLearningAttempts = 5;
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: postLearningAttempts,
                bestScore: 10
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ScoreInformation.Should().Be("(10% - 5 attempts)");
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
            postLearningAssessmentViewModel.NextSectionId.Should().Be(nextSectionId);
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

        [Test]
        public void Post_learning_assessment_should_not_have_onlyItemInOnlyCourse_when_other_sections_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: true,
                otherItemsInSectionExist: false
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.OnlyItemInOnlyCourse.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_should_not_have_onlyItemInOnlyCourse_when_other_items_in_section_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: false,
                otherItemsInSectionExist: true
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.OnlyItemInOnlyCourse.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_should_have_onlyItemInOnlyCourse()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: false,
                otherItemsInSectionExist: false
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.OnlyItemInOnlyCourse.Should().BeTrue();
        }

        [Test]
        public void Post_learning_assessment_should_have_noOtherItemsInSectionExist_when_no_other_sections_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherItemsInSectionExist: false
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.NoOtherItemsInSectionExist.Should().BeTrue();
        }

        [Test]
        public void Post_learning_assessment_should_not_have_noOtherItemsInSectionExist_when_other_sections_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherItemsInSectionExist: true
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.NoOtherItemsInSectionExist.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_should_show_completion_summary_when_include_certification_and_only_tutorial_and_section()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: false,
                otherItemsInSectionExist: false,
                includeCertification: true
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ShowCompletionSummary.Should().BeTrue();
        }

        [Test]
        public void Post_learning_assessment_should_not_show_completion_summary_when_include_certification_is_false()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: false,
                otherItemsInSectionExist: false,
                includeCertification: false
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_should_not_show_completion_summary_when_other_sections_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: true,
                otherItemsInSectionExist: false,
                includeCertification: true
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [Test]
        public void Post_learning_assessment_should_not_show_completion_summary_when_other_items_in_section_exist()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: false,
                otherItemsInSectionExist: true,
                includeCertification: true
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);


            // Then
            postLearningAssessmentViewModel.ShowCompletionSummary.Should().BeFalse();
        }

        [TestCase(2, "2020-12-25T15:00:00Z", 1, true, 75, 80, 85)]
        [TestCase(3, null, 0, true, 75, 80, 85)]
        [TestCase(4, null, 3, true, 75, 80, 85)]
        [TestCase(5, null, 3, false, 75, 80, 85)]
        [TestCase(6, null, 3, false, 75, 80, 0)]
        [TestCase(7, null, 3, false, 75, 0, 85)]
        [TestCase(8, null, 3, false, 75, 0, 0)]
        public void Post_learning_assessment_should_have_completion_summary_card_view_model(
            int customisationId,
            string? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            // Given
            var completedDateTime = completed != null ? DateTime.Parse(completed) : (DateTime?)null;

            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                completed: completedDateTime,
                maxPostLearningAssessmentAttempts: maxPostLearningAssessmentAttempts,
                isAssessed: isAssessed,
                postLearningAssessmentPassThreshold: postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold: diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold: tutorialsCompletionThreshold
            );

            var expectedCompletionSummaryViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                completedDateTime,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, customisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.CompletionSummaryCardViewModel
                .Should().BeEquivalentTo(expectedCompletionSummaryViewModel);
        }
    }
}
