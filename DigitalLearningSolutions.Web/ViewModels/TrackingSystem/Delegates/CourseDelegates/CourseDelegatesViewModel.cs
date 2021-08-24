namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CourseDelegatesViewModel
    {
        public CourseDelegatesViewModel(
            List<Course> courses,
            List<CourseDelegate> courseDelegates,
            int currentCustomisationId
        )
        {
            CustomisationId = currentCustomisationId;
            var courseOptions = courses.Select(c => (c.CustomisationId, c.CourseName));
            Courses = SelectListHelper.MapOptionsToSelectListItems(courseOptions, currentCustomisationId);

            Delegates = courseDelegates.Select(cd => new SearchableCourseDelegateViewModel(cd));
        }

        public int CustomisationId { get; set; }
        public IEnumerable<SelectListItem> Courses { get; set; }

        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }
    }
}
