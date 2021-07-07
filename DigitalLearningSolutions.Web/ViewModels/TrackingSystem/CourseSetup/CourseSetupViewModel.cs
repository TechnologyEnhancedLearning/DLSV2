namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseSetupViewModel
    {
        public CourseSetupViewModel(IEnumerable<CourseStatistics> courses)
        {
            Courses = courses.Select(course => new SearchableCourseStatisticsViewModel(course));
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }
    }
}
