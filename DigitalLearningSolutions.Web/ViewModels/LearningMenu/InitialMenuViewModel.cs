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
        public string Description { get; }
        public string? AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public bool ShouldShowCompletionSummary { get; }
        public IEnumerable<SectionCardViewModel> Sections { get; }
        public CompletionSummaryCardViewModel CompletionSummaryCardViewModel { get; }
        public bool ShowTime { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Id = courseContent.Id;
            Title = courseContent.Title;
            Description = courseContent.Description;
            AverageDuration = DurationFormattingHelper.FormatNullableDuration(courseContent.AverageDuration);

            CentreName = courseContent.CentreName;
            BannerText = courseContent.BannerText;
            ShouldShowCompletionSummary = courseContent.IncludeCertification;
            Sections = courseContent.Sections.Select(section => new SectionCardViewModel(
                section,
                Id,
                courseContent.CourseSettings.ShowPercentage
            ));
            CompletionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                courseContent.Id,
                courseContent.Completed,
                courseContent.MaxPostLearningAssessmentAttempts,
                courseContent.IsAssessed,
                courseContent.PostLearningAssessmentPassThreshold,
                courseContent.DiagnosticAssessmentCompletionThreshold,
                courseContent.TutorialsCompletionThreshold
            );
            ShowTime = AverageDuration != null && courseContent.CourseSettings.ShowTime;
        }
    }
}
