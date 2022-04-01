﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DateHelper = Helpers.DateHelper;

    public class DetailedCourseProgressViewModel
    {
        public DetailedCourseProgressViewModel(
            DetailedCourseProgress progress,
            DelegateAccessRoute accessedVia,
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
            ProfessionalRegistrationNumber = DisplayStringHelper.GetPrnDisplayString(
                progress.HasBeenPromptedForPrn,
                progress.ProfessionalRegistrationNumber
            );

            LastUpdated = progress.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Enrolled = progress.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
            CompleteBy = progress.CompleteBy?.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = progress.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);

            DiagnosticScore = progress.DiagnosticScore;

            ProgressDownloadUrl = currentSystemBaseUrl + $"/tracking/summary?ProgressID={progress.ProgressId}";

            Sections = progress.Sections.Select(s => new SectionProgressViewModel(s));
        }

        public DelegateAccessRoute AccessedVia { get; set; }
        public int ProgressId { get; set; }
        public int DelegateId { get; set; }
        public int CustomisationId { get; set; }
        public string ProgressDownloadUrl { get; set; }

        public string DelegateName { get; set; }
        public string? DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }

        public string LastUpdated { get; set; }
        public string Enrolled { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }

        public int? DiagnosticScore { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
