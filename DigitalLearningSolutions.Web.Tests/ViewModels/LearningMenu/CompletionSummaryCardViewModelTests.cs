namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    class CompletionSummaryCardViewModelTests
    {
        private const int CustomisationId = 1;
        private readonly DateTime? Completed = null;
        private const int MaxPostLearningAssessmentAttempts = 0;
        private const bool IsAssessed = true;
        private const int PostLearningAssessmentPassThreshold = 100;
        private const int DiagnosticAssessmentCompletionThreshold = 85;
        private const int TutorialsCompletionThreshold = 0;

        [Test]
        public void Completion_summary_card_should_have_customisationId()
        {
            // Given
            const int customisationId = 121;

            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                Completed,
                MaxPostLearningAssessmentAttempts,
                IsAssessed,
                PostLearningAssessmentPassThreshold,
                DiagnosticAssessmentCompletionThreshold,
                TutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void Completion_summary_card_completion_status_for_completed_should_be_complete()
        {
            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                CustomisationId,
                DateTime.Now,
                MaxPostLearningAssessmentAttempts,
                IsAssessed,
                PostLearningAssessmentPassThreshold,
                DiagnosticAssessmentCompletionThreshold,
                TutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CompletionStatus.Should().Be("Complete");
        }

        [Test]
        public void Completion_summary_card_completion_status_for_null_completed_should_be_incomplete()
        {
            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                CustomisationId,
                null,
                MaxPostLearningAssessmentAttempts,
                IsAssessed,
                PostLearningAssessmentPassThreshold,
                DiagnosticAssessmentCompletionThreshold,
                TutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CompletionStatus.Should().Be("Incomplete");
        }

        [Test]
        public void Completion_summary_card_completion_styling_for_completed_should_be_complete()
        {
            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                CustomisationId,
                DateTime.Now,
                MaxPostLearningAssessmentAttempts,
                IsAssessed,
                PostLearningAssessmentPassThreshold,
                DiagnosticAssessmentCompletionThreshold,
                TutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CompletionStyling.Should().Be("complete");
        }

        [Test]
        public void Completion_summary_card_completion_styling_for_null_completed_should_be_incomplete()
        {
            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                CustomisationId,
                null,
                MaxPostLearningAssessmentAttempts,
                IsAssessed,
                PostLearningAssessmentPassThreshold,
                DiagnosticAssessmentCompletionThreshold,
                TutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CompletionStyling.Should().Be("incomplete");
        }

        [TestCase(
            "2020-12-25T15:00:00Z",
            1,
            true,
            75,
            80,
            85,
            "You completed this course on 25 December 2020."
        )]
        [TestCase(
            null,
            0,
            true,
            75,
            80,
            85,
            "To complete this course, you must pass all post learning assessments with a score of 75% or higher."
        )]
        [TestCase(
            null,
            3,
            true,
            75,
            80,
            85,
            "To complete this course, you must pass all post learning assessments with a score of 75% or higher. Failing an assessment 3 times will lock your progress."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            80,
            85,
            "To complete this course, you must achieve 80% in the diagnostic assessment and complete 85% of the learning material."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            80,
            0,
            "To complete this course, you must achieve 80% in the diagnostic assessment."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            0,
            85,
            "To complete this course, you must complete 85% of the learning material."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            0,
            0,
            "There are no requirements to complete this course."
        )]
        public void Completion_summary_card_completion_should_have_formatted_completion_summary(
            string? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            string expectedSummary
        )
        {
            // When
            var completionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                CustomisationId,
                completed != null ? DateTime.Parse(completed) : (DateTime?)null,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold
            );

            // Then
            completionSummaryCardViewModel.CompletionSummary.Should().Be(expectedSummary);
        }
    }
}
