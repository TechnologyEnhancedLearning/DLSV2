namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;

    public class SelectedCourseDetailsViewModel : BaseSearchablePageViewModel<CourseDelegate>
    {
        public SelectedCourseDetailsViewModel(
            SearchSortFilterPaginationResult<CourseDelegate> result,
            IEnumerable<FilterModel> availableFilters,
            CourseDelegatesData courseDelegatesData,
            Dictionary<string, string> routeData
        ) : base(result, true, availableFilters, routeData: routeData)
        {
            var currentCourse =
                courseDelegatesData.Courses.Single(c => c.CustomisationId == courseDelegatesData.CustomisationId);
            Active = currentCourse.Active;
            var adminFieldsWithOptions = courseDelegatesData.CourseAdminFields.Where(field => field.Options.Count > 0);
            Delegates = result.ItemsToDisplay.Select(
                d => new DelegateCourseInfoViewModel(
                    d,
                    DelegateAccessRoute.CourseDelegates,
                    result.GetReturnPageQuery($"{d.DelegateId}-card"),
                    currentCourse.CourseName
                )
            );
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(
                courseDelegatesData.CourseAdminFields
            );
        }

        public bool Active { get; set; }
        public IEnumerable<DelegateCourseInfoViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } =
            Enumeration.GetAll<CourseDelegatesSortByOption>().Select(o => (o.DisplayText, o.PropertyName));

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
