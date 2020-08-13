namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class CompletedCourseViewModel
    {
        public string Name { get; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public DateTime StartedDate { get; }
        public DateTime CompletedDate { get; }
        public DateTime EvaluatedDate { get; }
        public int? DiagnosticScore { get; }
        public int PassedSections { get; }
        public int Sections { get; }
        public int ProgressId { get; }
        public string LaunchUrl { get; }

        public CompletedCourseViewModel(CompletedCourse course, IConfiguration config)
        {
            Name = course.CourseName;
            Id = course.CustomisationID;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            StartedDate = course.StartedDate;
            CompletedDate = course.Completed;
            EvaluatedDate = course.Evaluated;
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
