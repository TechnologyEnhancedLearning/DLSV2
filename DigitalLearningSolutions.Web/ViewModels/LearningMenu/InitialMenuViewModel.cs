namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseContent;

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
        private DateTime? Completed { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Id = courseContent.Id;
            Title = courseContent.Title;
            AverageDuration = courseContent.AverageDuration;
            CentreName = courseContent.CentreName;
            BannerText = courseContent.BannerText;
            ShouldShowCompletionSummary = courseContent.IncludeCertification;
            Completed = courseContent.Completed;
            Sections = courseContent.Sections.Select(section => new SectionCardViewModel(section));
            CompletionStatus = Completed == null ? "Incomplete" : "Complete";
            CompletionStyling = Completed == null ? "incomplete" : "complete";
        }
    }
}
