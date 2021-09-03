namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CourseDelegatesViewModel
    {
        public CourseDelegatesViewModel(CourseDelegatesData courseDelegatesData)
        {
            CustomisationId = courseDelegatesData.CustomisationId;

            var courseOptions = courseDelegatesData.Courses
                .Select(c => (c.CustomisationId, c.CourseNameWithInactiveFlag));
            Courses = SelectListHelper.MapOptionsToSelectListItems(courseOptions, courseDelegatesData.CustomisationId);

            // TODO: HEEDLS-564 - paginate properly instead of taking 10.
            var delegates = courseDelegatesData.Delegates.Take(10)
                .Select(cd => new SearchableCourseDelegateViewModel(cd));
            CourseDetails = courseDelegatesData.CustomisationId.HasValue
                ? new SelectedCourseDetails(
                    courseDelegatesData.CustomisationId.Value,
                    courseDelegatesData.Courses,
                    delegates
                )
                : null;
        }

        public int? CustomisationId { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }

        public SelectedCourseDetails? CourseDetails { get; set; }
    }
}
