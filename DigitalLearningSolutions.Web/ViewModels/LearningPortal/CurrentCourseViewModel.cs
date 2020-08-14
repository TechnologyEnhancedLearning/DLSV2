namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class CurrentCourseViewModel : NamedItemViewModel
    {
        public string Name { get; }
        public int Id { get; }
        public bool HasDiagnosticAssessment { get; }
        public bool HasLearningContent { get; }
        public bool HasLearningAssessmentAndCertification { get; }
        public DateTime StartedDate { get; }
        public DateTime LastAccessedDate { get; }
        public DateTime? CompleteByDate { get; }
        public int? DiagnosticScore { get; }
        public int PassedSections { get; }
        public int Sections { get; }
        public bool UserIsSupervisor { get; }
        public bool IsEnrolledWithGroup { get; }
        public int ProgressId { get; }
        public bool IsLocked { get; }
        public bool SelfEnrolled { get; }
        public string LaunchUrl { get; }
        public DateValidator.ValidationResult? CompleteByValidationResult { get; set; }

        public CurrentCourseViewModel(CurrentCourse course, IConfiguration config)
        {
            Name = course.CourseName;
            Id = course.CustomisationID;
            HasDiagnosticAssessment = course.HasDiagnostic;
            HasLearningContent = course.HasLearning;
            HasLearningAssessmentAndCertification = course.IsAssessed;
            StartedDate = course.StartedDate;
            LastAccessedDate = course.LastAccessed;
            CompleteByDate = course.CompleteByDate;
            DiagnosticScore = course.DiagnosticScore;
            PassedSections = course.Passes;
            Sections = course.Sections;
            UserIsSupervisor = course.SupervisorAdminId != 0;
            IsEnrolledWithGroup = course.GroupCustomisationId != 0;
            ProgressId = course.ProgressID;
            SelfEnrolled = course.EnrollmentMethodID == 1;
            IsLocked = course.PLLocked;
            LaunchUrl = config.GetLaunchUrl(course.CustomisationID);
        }

        public string DateStyle()
        {
            if (CompleteByDate < DateTime.Today)
            {
                return "overdue";
            }

            if (CompleteByDate < (DateTime.Today + TimeSpan.FromDays(30)))
            {
                return "due-soon";
            }

            return "";
        }

        public string DueByDescription()
        {
            return DateStyle() switch
            {
                "overdue" => "Overdue:",
                "due-soon" => "Due soon:",
                _ => ""
            };
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
