namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class CourseCompletionViewModel
    {
        public int CustomisationId { get; }
        public string CourseTitle { get; }
        public bool IsAssessed { get; }
        public int? DiagnosticScore { get; }
        public int PercentageTutorialsCompleted { get; }
        public int PostLearningPasses { get; }
        public int SectionCount { get; }

        public string CompletionStatus { get; }
        public bool ShowDiagnosticScore { get; }
        public bool ShowPercentageTutorialsCompleted { get; }
        public string? FinaliseText { get; }
        public string? FinaliseAriaLabel { get; }
        public string SummaryText { get; }
        public string DownloadSummaryUrl { get; }
        public string FinaliseUrl { get; }

        public CourseCompletionViewModel(IConfiguration config, CourseCompletion courseCompletion, int progressId)
        {
            CustomisationId = courseCompletion.Id;
            CourseTitle = courseCompletion.CourseTitle;
            IsAssessed = courseCompletion.IsAssessed;
            DiagnosticScore = courseCompletion.DiagnosticScore;
            PostLearningPasses = courseCompletion.PostLearningPasses;
            SectionCount = courseCompletion.SectionCount;

            PercentageTutorialsCompleted = Convert.ToInt32(Math.Floor(courseCompletion.PercentageTutorialsCompleted));

            CompletionStatus = courseCompletion.Completed == null ? "incomplete" : "complete";
            ShowDiagnosticScore = DiagnosticScore != null && courseCompletion.DiagnosticAttempts > 0;
            ShowPercentageTutorialsCompleted = PercentageTutorialsCompleted > 0;

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

            DownloadSummaryUrl = config.GetDownloadSummaryUrl(progressId);
            FinaliseUrl = config.GetEvaluateUrl(progressId, false);
        }

        private string? GetEvaluationOrCertificateText(DateTime? completed, DateTime? evaluated, bool isAssessed)
        {
            if (completed == null)
            {
                return null;
            }

            if (evaluated == null)
            {
                return "Evaluate";
            }

            return isAssessed ? "Certificate" : null;
        }
    }
}
