namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CourseDelegatesViewModel
    {
        public CourseDelegatesViewModel(
            CourseDelegatesData courseDelegatesData,
            SearchSortFilterPaginationResult<CourseDelegate> result,
            IEnumerable<FilterModel> availableFilters,
            string customisationIdQueryParameterName
        )
        {
            CustomisationId = courseDelegatesData.CustomisationId;

            var courseOptions = courseDelegatesData.Courses
                .Select(c => (c.CustomisationId, c.CourseNameWithInactiveFlag));
            Courses = SelectListHelper.MapOptionsToSelectListItems(courseOptions, courseDelegatesData.CustomisationId);

            CourseDetails = courseDelegatesData.CustomisationId.HasValue
                ? new SelectedCourseDetailsViewModel(
                    result,
                    availableFilters,
                    courseDelegatesData,
                    new Dictionary<string, string>
                        { { customisationIdQueryParameterName, courseDelegatesData.CustomisationId.Value.ToString() } }
                )
                : null;
        }

        public int? CustomisationId { get; set; }
        public IEnumerable<SelectListItem> Courses { get; set; }
        public SelectedCourseDetailsViewModel? CourseDetails { get; set; }
    }
}
