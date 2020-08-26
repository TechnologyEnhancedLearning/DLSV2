namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;

    public static class SortingHelper
    {
        public static IEnumerable<NamedItem> SortAllItems(
            IEnumerable<BaseCourse> courses,
            SelfAssessment? selfAssessment,
            string sortBy,
            string sortDirection
        )
        {
            if (sortBy == SortByOptionTexts.CourseName)
            {
                return SortByName(courses, selfAssessment, sortDirection);
            }

            IEnumerable<NamedItem> sortedCourses = SortCourses(courses, sortBy, sortDirection);
            if (selfAssessment == null)
            {
                return sortedCourses;
            }

            return sortDirection == BaseCoursePageViewModel.DescendingText
                ? sortedCourses.Append(selfAssessment)
                : sortedCourses.Prepend(selfAssessment);
        }
        private static IEnumerable<NamedItem> SortByName(
            IEnumerable<BaseCourse> courses,
            SelfAssessment? selfAssessment,
            string sortDirection
        )
        {
            var allItems = new List<NamedItem>(courses);
            if (selfAssessment != null)
            {
                allItems.Add(selfAssessment);
            }

            return sortDirection == BaseCoursePageViewModel.DescendingText
                ? allItems.OrderByDescending(course => course.Name)
                : allItems.OrderBy(course => course.Name);
        }

        private static IEnumerable<BaseCourse> SortCourses(IEnumerable<BaseCourse> courses, string sortBy, string sortDirection)
        {
            return sortBy switch
            {
                SortByOptionTexts.StartedDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<StartedCourse>().OrderByDescending(course => course.StartedDate)
                    : courses.Cast<StartedCourse>().OrderBy(course => course.StartedDate),
                SortByOptionTexts.LastAccessed => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<StartedCourse>().OrderByDescending(course => course.LastAccessed)
                    : courses.Cast<StartedCourse>().OrderBy(course => course.LastAccessed),
                SortByOptionTexts.DiagnosticScore => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<StartedCourse>().OrderByDescending(course => course.HasDiagnostic)
                        .ThenByDescending(course => course.DiagnosticScore)
                    : courses.Cast<StartedCourse>().OrderBy(course => course.HasDiagnostic)
                        .ThenBy(course => course.DiagnosticScore),
                SortByOptionTexts.PassedSections => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<StartedCourse>().OrderByDescending(course => course.IsAssessed)
                        .ThenByDescending(course => course.Passes)
                    : courses.Cast<StartedCourse>().OrderBy(course => course.IsAssessed)
                        .ThenBy(course => course.Passes),
                SortByOptionTexts.CompletedDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<CompletedCourse>().OrderByDescending(course => course.Completed)
                    : courses.Cast<CompletedCourse>().OrderBy(course => course.Completed),
                SortByOptionTexts.CompleteByDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<CurrentCourse>().OrderByDescending(course => course.CompleteByDate)
                    : courses.Cast<CurrentCourse>().OrderBy(course => course.CompleteByDate),
                SortByOptionTexts.Brand => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<AvailableCourse>().OrderByDescending(course => course.Brand)
                    : courses.Cast<AvailableCourse>().OrderBy(course => course.Brand),
                SortByOptionTexts.Category => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<AvailableCourse>().OrderByDescending(course => course.Category)
                    : courses.Cast<AvailableCourse>().OrderBy(course => course.Category),
                SortByOptionTexts.Topic => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? courses.Cast<AvailableCourse>().OrderByDescending(course => course.Topic)
                    : courses.Cast<AvailableCourse>().OrderBy(course => course.Topic),
                _ => courses
            };
        }
    }
}
