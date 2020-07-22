namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CurrentViewModel
    {
        private readonly IEnumerable<CurrentCourse> currentCourses;
        private readonly int candidateId;

        public CurrentViewModel(IEnumerable<CurrentCourse> currentCourses, int candidateId)
        {
            this.currentCourses = currentCourses;
            this.candidateId = candidateId;
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel
                {
                    Name = c.CourseName,
                    Id = c.CustomisationID,
                    HasDiagnostic = c.HasDiagnostic,
                    HasLearning = c.HasLearning,
                    IsAssessed = c.IsAssessed,
                    StartedDate = c.StartedDate,
                    LastAccessed = c.LastAccessed,
                    CompleteByDate = c.CompleteByDate,
                    DiagnosticScore = c.DiagnosticScore,
                    Passes = c.Passes,
                    Sections = c.Sections,
                    UserIsSupervisor = c.SupervisorAdminId == candidateId,
                    IsEnrolledWithGroup = c.GroupCustomisationId != 0,
    });
            }
        }

        public class CurrentCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public bool HasDiagnostic { get; set; }
            public bool HasLearning { get; set; }
            public bool IsAssessed { get; set; }
            public DateTime StartedDate { get; set; }
            public DateTime LastAccessed { get; set; }
            public DateTime CompleteByDate { get; set; }
            public int? DiagnosticScore { get; set; }
            public int Passes { get; set; }
            public int Sections { get; set; }
            public bool UserIsSupervisor { get; set; }
            public bool IsEnrolledWithGroup { get; set; }
        }
    }
}
