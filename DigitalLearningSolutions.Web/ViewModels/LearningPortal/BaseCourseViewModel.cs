namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseCourseViewModel : NamedItemViewModel
    {
        public string Name { get; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public DateTime StartedDate { get; }
        public int? DiagnosticScore { get; }
        public int PassedSections { get; }
        public int Sections { get; }
        public int ProgressId { get; }
        public string LaunchUrl { get; }

        protected BaseCourseViewModel(BaseCourse course, IConfiguration config)
        {
            Name = course.CourseName;
            Id = course.CustomisationID;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            StartedDate = course.StartedDate;
            DiagnosticScore = course.DiagnosticScore;
            PassedSections = course.Passes;
            Sections = course.Sections;
            ProgressId = course.ProgressID;
            LaunchUrl = config.GetLaunchUrl(course.CustomisationID);
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
