namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;

    public class CompletionSummaryCardViewModel
    {
        public int CustomisationId { get; }
        public string CompletionStatus { get; }
        public string CompletionStyling { get; }
        public string CompletionSummary { get; }

        public CompletionSummaryCardViewModel(
            int customisationId,
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            CustomisationId = customisationId;
            CompletionStatus = completed == null ? "Incomplete" : "Complete";
            CompletionStyling = completed == null ? "incomplete" : "complete";
            CompletionSummary = CompletionSummaryHelper.GetCompletionSummary(
                completed,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold
            );
        }
    }
}
