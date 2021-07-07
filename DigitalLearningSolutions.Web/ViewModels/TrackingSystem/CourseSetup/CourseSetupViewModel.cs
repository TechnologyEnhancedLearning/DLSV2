namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseSetupViewModel
    {
        public CourseSetupViewModel(IEnumerable<CourseStatistics> courses)
        {
            Courses = courses;
        }

        public IEnumerable<CourseStatistics> Courses { get; set; }
    }
}
