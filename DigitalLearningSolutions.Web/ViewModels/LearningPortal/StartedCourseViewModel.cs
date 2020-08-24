namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Configuration;

    public abstract class StartedCourseViewModel : BaseCourseViewModel
    {
        public DateTime StartedDate { get; }
        public DateTime LastAccessedDate { get; }
        public int? DiagnosticScore { get; }
        public int PassedSections { get; }
        public int Sections { get; }
        public int ProgressId { get; }

        protected StartedCourseViewModel(StartedCourse course, IConfiguration config) : base(course, config)
        {
            StartedDate = course.StartedDate;
            LastAccessedDate = course.LastAccessed;
            DiagnosticScore = course.DiagnosticScore;
            PassedSections = course.Passes;
            Sections = course.Sections;
            ProgressId = course.ProgressID;
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
