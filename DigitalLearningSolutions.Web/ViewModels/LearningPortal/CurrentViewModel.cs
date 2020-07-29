namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Configuration;

    public class CurrentViewModel
    {
        private readonly IEnumerable<CurrentCourse> currentCourses;
        private readonly IConfiguration config;

        public CurrentViewModel(IEnumerable<CurrentCourse> currentCourses, IConfiguration config)
        {
            this.currentCourses = currentCourses;
            this.config = config;
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel(c, config));
            }
        }

        public class CurrentCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public bool HasDiagnosticAssessment { get; set; }
            public bool HasLearningContent { get; set; }
            public bool HasLearningAssessmentAndCertification { get; set; }
            public DateTime StartedDate { get; set; }
            public DateTime LastAccessedDate { get; set; }
            public DateTime? CompleteByDate { get; set; }
            public int? DiagnosticScore { get; set; }
            public int PassedSections { get; set; }
            public int Sections { get; set; }
            public bool UserIsSupervisor { get; set; }
            public bool IsEnrolledWithGroup { get; set; }
            public int ProgressId { get; set; }
            public string LaunchUrl { get; set; }

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
                LaunchUrl = $"{config["CurrentSystemBaseUrl"]}/tracking/learn?CustomisationID={course.CustomisationID}&lp=1";
            }

            public bool Overdue()
            {
                return CompleteByDate <= DateTime.Today;
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
}
