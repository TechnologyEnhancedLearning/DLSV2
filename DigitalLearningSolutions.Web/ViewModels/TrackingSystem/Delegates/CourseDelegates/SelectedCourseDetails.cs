namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class SelectedCourseDetails
    {
        public SelectedCourseDetails(int customisationId, IEnumerable<Course> courses, IEnumerable<SearchableCourseDelegateViewModel> delegates)
        {
            Active = courses.Single(c => c.CustomisationId == customisationId).Active;
            Delegates = delegates;
        }
        
        public bool Active { get; set; }

        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }
    }
}
