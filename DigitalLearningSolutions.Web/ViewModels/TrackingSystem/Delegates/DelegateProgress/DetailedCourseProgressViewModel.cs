namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class DetailedCourseProgressViewModel
    {
        public DetailedCourseProgressViewModel(
            DetailedCourseProgress progress,
            DelegateProgressAccessRoute accessedVia,
            string currentSystemBaseUrl
        )
        {
            AccessedVia = accessedVia;
            ProgressId = progress.ProgressId;
            DelegateId = progress.DelegateId;
            CustomisationId = progress.CustomisationId;

            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                progress.DelegateFirstName,
                progress.DelegateLastName
            );
            DelegateEmail = progress.DelegateEmail;
            DelegateNumber = progress.DelegateNumber;

            LastUpdated = progress.LastUpdated.ToString(DateHelper.StandardDateFormat);
            Enrolled = progress.Enrolled.ToString(DateHelper.StandardDateFormat);
            CompleteBy = progress.CompleteBy?.ToString(DateHelper.StandardDateFormat);
            Completed = progress.Completed?.ToString(DateHelper.StandardDateFormat);

            DiagnosticScore = progress.DiagnosticScore;

            ProgressDownloadUrl = currentSystemBaseUrl + $"/tracking/summary?ProgressID={progress.ProgressId}";

            Sections = progress.Sections.Select(s => new SectionProgressViewModel(s));
        }

        public DelegateProgressAccessRoute AccessedVia { get; set; }
        public int ProgressId { get; set; }
        public int DelegateId { get; set; }
        public int CustomisationId { get; set; }
        public string ProgressDownloadUrl { get; set; }

        public string DelegateName { get; set; }
        public string? DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }

        public string LastUpdated { get; set; }
        public string Enrolled { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }

        public int? DiagnosticScore { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
