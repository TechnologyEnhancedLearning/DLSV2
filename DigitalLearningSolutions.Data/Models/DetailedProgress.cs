namespace DigitalLearningSolutions.Data.Models
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class DetailedCourseProgress
    {
        public DetailedCourseProgress(
            Progress progress,
            IEnumerable<DetailedSectionProgress> sections,
            DelegateCourseInfo delegateCourseInfo
        )
        {
            DiagnosticScore = progress.DiagnosticScore;
            ProgressId = progress.ProgressId;
            CustomisationId = progress.CustomisationId;
            DelegateId = progress.CandidateId;

            DelegateFirstName = delegateCourseInfo.DelegateFirstName;
            DelegateLastName = delegateCourseInfo.DelegateLastName;
            DelegateEmail = delegateCourseInfo.DelegateEmail;
            DelegateNumber = delegateCourseInfo.DelegateNumber;
            HasBeenPromptedForPrn = delegateCourseInfo.HasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = delegateCourseInfo.ProfessionalRegistrationNumber;

            LastUpdated = delegateCourseInfo.LastUpdated;
            Enrolled = delegateCourseInfo.Enrolled;
            CompleteBy = delegateCourseInfo.CompleteBy;
            Completed = delegateCourseInfo.Completed;

            Sections = sections;
        }

        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int DelegateId { get; set; }
        public int? DiagnosticScore { get; set; }

        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }

        public DateTime LastUpdated { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }

        public IEnumerable<DetailedSectionProgress> Sections { get; set; }
    }

    public class DetailedSectionProgress
    {
        public string SectionName { get; set; }
        public int SectionId { get; set; }

        public int Completion { get; set; }
        public int TotalTime { get; set; }
        public int AverageTime { get; set; }
        public string? PostLearningAssessPath { get; set; }
        public bool IsAssessed { get; set; }
        public int Outcome { get; set; }
        public int Attempts { get; set; }
        public bool Passed { get; set; }

        public IEnumerable<DetailedTutorialProgress>? Tutorials { get; set; }
    }

    public class DetailedTutorialProgress
    {
        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public int PossibleScore { get; set; }
    }
}
