namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;
    using DigitalLearningSolutions.Web.Helpers;

    public class CourseCompletionViewModel
    {
        public int CustomisationId { get; }
        public string CourseTitle { get; }
        public bool IsAssessed { get; }
        public int? DiagnosticScore { get; }
        public int DiagnosticAttempts { get; }
        public int PercentageTutorialsCompleted { get; }
        public int PostLearningPasses { get; }
        public int SectionCount { get; }

        public string CompletionStatus { get; }
        public string? FinaliseText { get; }
        public string? FinaliseAriaLabel { get; }
        public string SummaryText { get; }

        public CourseCompletionViewModel(CourseCompletion courseCompletion)
        {
            CustomisationId = courseCompletion.Id;
            CourseTitle = courseCompletion.CourseTitle;
            IsAssessed = courseCompletion.IsAssessed;
            DiagnosticScore = courseCompletion.DiagnosticScore;
            DiagnosticAttempts = courseCompletion.DiagnosticAttempts;
            PercentageTutorialsCompleted = courseCompletion.PercentageTutorialsCompleted;
            PostLearningPasses = courseCompletion.PostLearningPasses;
            SectionCount = courseCompletion.SectionCount;

            CompletionStatus = courseCompletion.Completed == null ? "incomplete" : "complete";

            FinaliseText = GetEvaluationOrCertificateText(
                courseCompletion.Completed,
                courseCompletion.Evaluated,
                courseCompletion.IsAssessed
            );

            FinaliseAriaLabel = FinaliseText switch
            {
                "Evaluate" => "Evaluate course",
                "Certificate" => "View or print certificate",
                _ => null
            };

            SummaryText = CompletionSummaryHelper.GetCompletionSummary(
                courseCompletion.Completed,
                courseCompletion.MaxPostLearningAssessmentAttempts,
                courseCompletion.IsAssessed,
                courseCompletion.PostLearningAssessmentPassThreshold,
                courseCompletion.DiagnosticAssessmentCompletionThreshold,
                courseCompletion.TutorialsCompletionThreshold
            );
        }

        private string? GetEvaluationOrCertificateText(DateTime? completed, DateTime? evaluated, bool isAssessed)
        {
            if (completed == null)
            {
                return null;
            }

            if (isAssessed)
            {
                return "Certificate";
            }

            return evaluated == null ? "Evaluate" : null;
        }
    }
}
