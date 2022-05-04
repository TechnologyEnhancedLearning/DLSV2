namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectedCourseDetailsViewModel : BaseSearchablePageViewModel<CourseDelegate>
    {
        public SelectedCourseDetailsViewModel(
            SearchSortFilterPaginationResult<CourseDelegate> result,
            IEnumerable<FilterModel> availableFilters,
            CourseDelegatesData courseDelegatesData,
            Dictionary<string, string> routeData
        ) : base(result, true, availableFilters, routeData: routeData)
        {
            Active = courseDelegatesData.Courses.Single(c => c.CustomisationId == courseDelegatesData.CustomisationId)
                .Active;
            var adminFieldsWithOptions = courseDelegatesData.CourseAdminFields.Where(field => field.Options.Count > 0);
            Delegates = result.ItemsToDisplay.Select(
                d =>
                {
                    var adminFields = AdminFieldsHelper.GetCourseAdminFieldViewModels(
                        d,
                        courseDelegatesData.CourseAdminFields
                    );
                    return new SearchableCourseDelegateViewModel(d, adminFields, adminFieldsWithOptions, result.GetReturnPageQuery($"{d.DelegateId}-card"));
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
