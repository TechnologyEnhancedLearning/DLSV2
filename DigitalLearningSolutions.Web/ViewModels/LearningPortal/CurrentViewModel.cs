namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CurrentViewModel
    {
        private readonly IEnumerable<CurrentCourse> currentCourses;
        private readonly IConfiguration config;

        [BindProperty]
        public string SortDirection { get; set; }

        [BindProperty]
        public string SortBy { get; set; }
        public readonly SelectList SortByOptions = new SelectList(new[] {
            SortByOptionTexts.CourseName,
            SortByOptionTexts.StartedDate,
            SortByOptionTexts.LastAccessed,
            SortByOptionTexts.CompleteByDate,
            SortByOptionTexts.DiagnosticScore,
            SortByOptionTexts.PassedSections
        });
        public readonly string AscendingText = "Ascending";
        public readonly string DescendingText = "Descending";

        public CurrentViewModel(IEnumerable<CurrentCourse> currentCourses, IConfiguration config, string sortBy, string sortDirection)
        {
            this.config = config;
            SortBy = sortBy;
            SortDirection = sortDirection;
            this.currentCourses = SortBy switch
            {
                LearningPortal.SortByOptionTexts.StartedDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.StartedDate)
                    : currentCourses.OrderBy((course) => course.StartedDate),
                LearningPortal.SortByOptionTexts.LastAccessed => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.LastAccessed)
                    : currentCourses.OrderBy((course) => course.LastAccessed),
                LearningPortal.SortByOptionTexts.CompleteByDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.CompleteByDate)
                    : currentCourses.OrderBy((course) => course.CompleteByDate),
                LearningPortal.SortByOptionTexts.DiagnosticScore => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.HasDiagnostic)
                        .ThenByDescending((course) => course.DiagnosticScore)
                    : currentCourses.OrderBy((course) => course.HasDiagnostic)
                        .ThenBy((course) => course.DiagnosticScore),
                LearningPortal.SortByOptionTexts.PassedSections => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.IsAssessed)
                        .ThenByDescending((course) => course.Passes)
                    : currentCourses.OrderBy((course) => course.IsAssessed)
                        .ThenBy((course) => course.Passes),
                LearningPortal.SortByOptionTexts.CourseName => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending((course) => course.CourseName)
                    : currentCourses.OrderBy((course) => course.CourseName),
                _ =>  currentCourses
            };
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

    public class SortByOptionTexts
    {
        public const string
            CourseName = "Course Name",
            StartedDate = "Enrolled Date",
            LastAccessed = "Last Accessed Date",
            CompleteByDate = "Complete By Date",
            DiagnosticScore = "Diagnostic Score",
            PassedSections = "Passed Sections";
    }

}
