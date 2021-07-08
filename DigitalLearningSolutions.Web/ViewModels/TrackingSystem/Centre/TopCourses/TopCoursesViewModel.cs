namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.TopCourses
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class TopCoursesViewModel
    {
        public TopCoursesViewModel(IEnumerable<CourseStatistics> topCourses)
        {
            TopCourses = topCourses;
        }
        public IEnumerable<CourseStatistics> TopCourses { get; set; }
    }
}
