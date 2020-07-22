namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CurrentViewModel
    {
        private readonly IEnumerable<CurrentCourse> currentCourses;

        public CurrentViewModel(IEnumerable<CurrentCourse> currentCourses)
        {
            this.currentCourses = currentCourses;
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel
                {
                    Name = c.CourseName,
                    Id = c.CustomisationID,
                    HasDiagnosticAssessment = c.HasDiagnostic,
                    HasLearningContent = c.HasLearning,
                    HasLearningAssessmentAndCertification = c.IsAssessed,
                    StartedDate = c.StartedDate,
                    LastAccessedDate = c.LastAccessed,
                    CompleteByDate = c.CompleteByDate,
                    DiagnosticScore = c.DiagnosticScore,
                    PassedSections = c.Passes,
                    Sections = c.Sections,
                    UserIsSupervisor = c.SupervisorAdminId != 0,
                    IsEnrolledWithGroup = c.GroupCustomisationId != 0,
    });
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
        }
    }
}
