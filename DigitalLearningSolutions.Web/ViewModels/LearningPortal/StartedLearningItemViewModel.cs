namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Configuration;

    public abstract class StartedLearningItemViewModel : BaseLearningItemViewModel
    {
        public DateTime StartedDate { get; }
        public DateTime? LastAccessedDate { get; }
        public int? DiagnosticScore { get; }
        public int PassedSections { get; }
        public int Sections { get; }
        public int ProgressId { get; }
        public string? CentreName { get; }

        protected StartedLearningItemViewModel(StartedLearningItem course) : base(course)
        {
            StartedDate = course.StartedDate;
            LastAccessedDate = course.LastAccessed;
            DiagnosticScore = course.DiagnosticScore;
            PassedSections = course.Passes;
            Sections = course.Sections;
            ProgressId = course.ProgressID;
            CentreName = course.CentreName;
        }

        public bool HasDiagnosticScore()
        {
            return HasDiagnosticAssessment && DiagnosticScore != null;
        }

        public bool HasPassedSections()
        {
            return HasLearningAssessmentAndCertification;
        }

        public string DisplayPassedSections()
        {
            return $"{PassedSections}/{Sections}";
        }
    }
}
