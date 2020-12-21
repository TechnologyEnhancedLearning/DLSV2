namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Web.Helpers;

    public class InitialMenuViewModel
    {
        public int Id { get; }
        public string Title { get; }
        public string AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public bool ShouldShowCompletionSummary { get; }
        public IEnumerable<SectionCardViewModel> Sections { get; }
        public string CompletionStatus { get; }
        public string CompletionStyling { get; }
        public string CompletionSummary { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Id = courseContent.Id;
            Title = courseContent.Title;
            AverageDuration = FormatDuration(courseContent.AverageDuration);

            CentreName = courseContent.CentreName;
            BannerText = courseContent.BannerText;
            ShouldShowCompletionSummary = courseContent.IncludeCertification;
            Sections = courseContent.Sections.Select(section => new SectionCardViewModel(section, Id));
            CompletionStatus = courseContent.Completed == null ? "Incomplete" : "Complete";
            CompletionStyling = courseContent.Completed == null ? "incomplete" : "complete";
            CompletionSummary = CompletionSummaryHelper.GetCompletionSummary(
                courseContent.Completed,
                courseContent.MaxPostLearningAssessmentAttempts,
                courseContent.IsAssessed,
                courseContent.PostLearningAssessmentPassThreshold,
                courseContent.DiagnosticAssessmentCompletionThreshold,
                courseContent.TutorialsCompletionThreshold
            );
        }

        private static string FormatDuration(int? duration)
        {
            if (duration == null)
            {
                return "Not applicable";
            }

            if (duration < 60)
            {
                return FormatTimeUnits(duration.Value, "minute");
            }

            var durationMinutes = duration.Value % 60;
            var formattedHours = FormatTimeUnits(duration.Value / 60, "hour");
            var formattedMinutes = FormatTimeUnits(durationMinutes, "minute");
            return durationMinutes == 0 ? formattedHours : $"{formattedHours} {formattedMinutes}";

        }

        private static string FormatTimeUnits(int duration, string unit) =>
            duration == 1 ? $"{duration} {unit}" : $"{duration} {unit}s";
    }
}
