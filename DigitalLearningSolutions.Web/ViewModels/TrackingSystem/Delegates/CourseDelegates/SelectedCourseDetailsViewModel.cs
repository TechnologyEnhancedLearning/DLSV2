namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectedCourseDetailsViewModel : BaseSearchablePageViewModel
    {
        public SelectedCourseDetailsViewModel(
            CourseDelegatesData courseDelegatesData,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page,
            Dictionary<string, string> routeData
        ) : base(null, page, true, sortBy, sortDirection, filterBy, routeData: routeData)
        {
            Active = courseDelegatesData.Courses.Single(c => c.CustomisationId == courseDelegatesData.CustomisationId)
                .Active;
            var courseDelegatesToShow = SortFilterAndPaginate(courseDelegatesData.Delegates);
            var adminFieldsWithOptions = courseDelegatesData.CourseAdminFields.Where(field => field.Options.Count > 0);
            Delegates = courseDelegatesToShow.Select(
                d =>
                {
                    var adminFields = AdminFieldsHelper.GetCourseAdminFieldViewModels(
                        d,
                        courseDelegatesData.CourseAdminFields
                    );
                    return new SearchableCourseDelegateViewModel(d, adminFields, adminFieldsWithOptions);
                }
            );
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(
                courseDelegatesData.CourseAdminFields
            );
        }

        public bool Active { get; set; }
        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } =
            Enumeration.GetAll<CourseDelegatesSortByOption>().Select(o => (o.DisplayText, o.PropertyName));

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
