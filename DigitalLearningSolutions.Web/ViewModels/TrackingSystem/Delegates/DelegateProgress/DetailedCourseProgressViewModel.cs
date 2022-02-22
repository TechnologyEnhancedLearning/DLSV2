namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class DetailedCourseProgressViewModel
    {
        public DetailedCourseProgressViewModel(
            DelegateUser delegateUser,
            DetailedCourseProgress progress,
            DelegateCourseDetails course,
            DelegateProgressAccessRoute accessedVia,
            string currentSystemBaseUrl
        )
        {
            AccessedVia = accessedVia;
            ProgressId = progress.ProgressId;
            DelegateId = delegateUser.Id;
            CustomisationId = progress.CustomisationId;

            DelegateName = delegateUser.FullName;
            DelegateEmail = delegateUser.EmailAddress;
            DelegateNumber = delegateUser.CandidateNumber;

            LastUpdated = course.DelegateCourseInfo.LastUpdated;
            Enrolled = course.DelegateCourseInfo.Enrolled;
            CompleteBy = course.DelegateCourseInfo.CompleteBy;
            Completed = course.DelegateCourseInfo.Completed;

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

        public DateTime LastUpdated { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }

        public int? DiagnosticScore { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
