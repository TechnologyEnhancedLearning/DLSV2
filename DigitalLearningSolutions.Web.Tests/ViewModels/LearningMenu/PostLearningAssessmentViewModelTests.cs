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
        public void Post_learning_assessment_assessment_status_with_no_attempts_should_have_not_passed_styling()
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
            postLearningAssessmentViewModel.AssessmentStatusStyling.Should().Be("not-passed-text");
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_passes_should_have_passed_styling()
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
            postLearningAssessmentViewModel.AssessmentStatusStyling.Should().Be("passed-text");
        }

        [Test]
        public void Post_learning_assessment_assessment_status_with_attempts_but_no_passes_should_have_not_passed_styling()
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
            postLearningAssessmentViewModel.AssessmentStatusStyling.Should().Be("not-passed-text");
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Post_learning_assessment_start_button_should_be_grey_if_it_has_been_attempted(int postLearningPasses)
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: 1,
                plPasses: postLearningPasses
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.StartButtonAdditionalStyling.Should().Be("nhsuk-button--secondary");
        }

        [Test]
        public void Post_learning_assessment_start_button_should_have_no_extra_colour_if_it_has_not_been_attempted()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: 0
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.StartButtonAdditionalStyling.Should().Be("");
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Post_learning_assessment_start_button_should_say_restart_if_it_has_been_attempted(int postLearningPasses)
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: 1,
                plPasses: postLearningPasses
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.StartButtonText.Should().Be("Restart assessment");
        }

        [Test]
        public void Post_learning_assessment_start_button_should_say_start_if_it_has_not_been_attempted()
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                attemptsPl: 0
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.StartButtonText.Should().Be("Start assessment");
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
            postLearningAssessmentViewModel.ScoreInformation.Should().Be("10% - 1 attempt");
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
            postLearningAssessmentViewModel.ScoreInformation.Should().Be("10% - 5 attempts");
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

        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        public void Post_learning_assessment_should_have_onlyItemInOnlySection(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInOnlySection
        )
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.OnlyItemInOnlySection.Should().Be(expectedOnlyItemInOnlySection);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Post_learning_assessment_should_have_onlyItemInThisSection(
            bool otherItemsInSectionExist,
            bool expectedOnlyItemInThisSection
        )
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherItemsInSectionExist: otherItemsInSectionExist
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.OnlyItemInThisSection.Should().Be(expectedOnlyItemInThisSection);
        }

        [TestCase(false, false, false, false)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(true, true, true, false)]
        public void Post_learning_assessment_should_have_showCompletionSummary(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            bool includeCertification,
            bool expectedShowCompletionSummary
        )
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist,
                includeCertification: includeCertification
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ShowCompletionSummary.Should().Be(expectedShowCompletionSummary);
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

        [TestCase(false, false, 0, false)]
        [TestCase(false, true, 0, false)]
        [TestCase(true, false, 0, false)]
        [TestCase(true, true, 0, false)]
        [TestCase(false, false, 1, false)]
        [TestCase(false, true, 1, true)]
        [TestCase(true, false, 1, true)]
        [TestCase(true, true, 1, true)]
        public void Post_learning_assessment_should_have_showNextButton(
            bool otherSectionsExist,
            bool otherItemsInSectionExist,
            int attemptsPl,
            bool expectedShowNextButton
        )
        {
            // Given
            var postLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment(
                otherSectionsExist: otherSectionsExist,
                otherItemsInSectionExist: otherItemsInSectionExist,
                attemptsPl: attemptsPl
            );

            // When
            var postLearningAssessmentViewModel =
                new PostLearningAssessmentViewModel(postLearningAssessment, CustomisationId, SectionId);

            // Then
            postLearningAssessmentViewModel.ShowNextButton.Should().Be(expectedShowNextButton);
        }
    }
}
