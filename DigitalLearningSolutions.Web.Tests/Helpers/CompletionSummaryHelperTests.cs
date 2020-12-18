namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    class CompletionSummaryHelperTests
    {
        [Test]
        public void GetCompletionSummary_should_return_summary_of_complete_course()
        {
            // Given
            DateTime? completedDate = DateTime.Parse("2020-12-25T15:00:00Z");
            const int maxAssessments = 1;
            const bool isAssessed = true;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 80;
            const int tutorialThreshold = 85;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be("You completed this course on 25 December 2020.");
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_assessed_course_without_threshold()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 0;
            const bool isAssessed = true;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 80;
            const int tutorialThreshold = 85;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be($"To complete this course, you must pass all post learning assessments with a score " +
                                $"of {postLearningThreshold}% or higher.");
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_assessed_course_with_threshold()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 3;
            const bool isAssessed = true;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 80;
            const int tutorialThreshold = 85;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be(
                $"To complete this course, you must pass all post learning assessments with a score of " +
                $"{postLearningThreshold}% or higher. Failing an assessment " +
                $"{maxAssessments} times will lock your progress.");
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_non_assessed_course_with_tutorial_and_post_learning_thresholds()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 3;
            const bool isAssessed = false;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 80;
            const int tutorialThreshold = 85;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be(
                $"To complete this course, you must achieve {diagnosticThreshold}% " +
                $"in the diagnostic assessment and complete {tutorialThreshold}% of the learning " +
                $"material.");
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_non_assessed_course_with_diagnostic_threshold()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 3;
            const bool isAssessed = false;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 80;
            const int tutorialThreshold = 0;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be(
                $"To complete this course, you must achieve {diagnosticThreshold}% in the diagnostic assessment."
            );
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_non_assessed_course_with_tutorial_threshold()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 3;
            const bool isAssessed = false;
            const int postLearningThreshold = 75;
            const int diagnosticThreshold = 0;
            const int tutorialThreshold = 85;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be(
                $"To complete this course, you must complete {tutorialThreshold}% of the learning " +
                $"material."
            );
        }

        [Test]
        public void GetCompletionSummary_should_return_summary_of_course_with_no_requirements()
        {
            // Given
            DateTime? completedDate = null;
            const int maxAssessments = 3;
            const bool isAssessed = false;
            const int postLearningThreshold = 0;
            const int diagnosticThreshold = 0;
            const int tutorialThreshold = 0;

            // When
            var summary = CompletionSummaryHelper.GetCompletionSummary(
                completedDate,
                maxAssessments,
                isAssessed,
                postLearningThreshold,
                diagnosticThreshold,
                tutorialThreshold
            );

            // Then
            summary.Should().Be("There are no requirements to complete this course.");
        }
    }
}
