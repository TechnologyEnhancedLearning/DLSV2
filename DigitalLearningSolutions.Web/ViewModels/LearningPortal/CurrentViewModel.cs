namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;

    public class CurrentViewModel
    {
        private readonly IEnumerable<CurrentCourse> currentCourses;
        private readonly IConfiguration config;

        [BindProperty] public string SortDirection { get; set; }

        [BindProperty] public string SortBy { get; set; }

        public readonly SelectList SortByOptions = new SelectList(new[]
        {
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
                SortByOptionTexts.StartedDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.StartedDate)
                    : currentCourses.OrderBy(course => course.StartedDate),
                SortByOptionTexts.LastAccessed => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.LastAccessed)
                    : currentCourses.OrderBy(course => course.LastAccessed),
                SortByOptionTexts.CompleteByDate => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.CompleteByDate)
                    : currentCourses.OrderBy(course => course.CompleteByDate),
                SortByOptionTexts.DiagnosticScore => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.HasDiagnostic)
                        .ThenByDescending(course => course.DiagnosticScore)
                    : currentCourses.OrderBy(course => course.HasDiagnostic)
                        .ThenBy(course => course.DiagnosticScore),
                SortByOptionTexts.PassedSections => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.IsAssessed)
                        .ThenByDescending(course => course.Passes)
                    : currentCourses.OrderBy(course => course.IsAssessed)
                        .ThenBy(course => course.Passes),
                SortByOptionTexts.CourseName => SortDirection == DescendingText
                    ? currentCourses.OrderByDescending(course => course.CourseName)
                    : currentCourses.OrderBy(course => course.CourseName),
                _ => currentCourses
            };
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel(c, config));
            }
        }
    }

    public static class SortByOptionTexts
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
