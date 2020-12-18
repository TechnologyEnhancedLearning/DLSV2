namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class CompletionSummaryHelper
    {
        public static string GetCompletionSummary(
            DateTime? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            if (completed != null)
            {
                return $"You completed this course on {completed:dd MMMM yyyy}.";
            }

            if (isAssessed)
            {
                return maxPostLearningAssessmentAttempts == 0
                    ? $"To complete this course, you must pass all post learning assessments with a score of " +
                      $"{postLearningAssessmentPassThreshold}% or higher."
                    : $"To complete this course, you must pass all post learning assessments with a score of " +
                      $"{postLearningAssessmentPassThreshold}% or higher. Failing an assessment " +
                      $"{maxPostLearningAssessmentAttempts} times will lock your progress.";
            }

            if (diagnosticAssessmentCompletionThreshold > 0 && tutorialsCompletionThreshold > 0)
            {
                return $"To complete this course, you must achieve {diagnosticAssessmentCompletionThreshold}% " +
                       $"in the diagnostic assessment and complete {tutorialsCompletionThreshold}% of the learning " +
                       $"material.";
            }

            if (tutorialsCompletionThreshold > 0)
            {
                return $"To complete this course, you must complete {tutorialsCompletionThreshold}% of the learning " +
                       $"material.";
            }

            if (diagnosticAssessmentCompletionThreshold > 0)
            {
                return $"To complete this course, you must achieve {diagnosticAssessmentCompletionThreshold}% in " +
                       $"the diagnostic assessment.";
            }

            return "There are no requirements to complete this course.";
        }
    }
}
