namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class DelegateCoursesViewModel : BaseSearchablePageViewModel<CourseStatisticsWithAdminFieldResponseCounts>
    {
        public DelegateCoursesViewModel(
            SearchSortFilterPaginationResult<CourseStatisticsWithAdminFieldResponseCounts> result,
            IEnumerable<FilterModel> availableFilters
        ) : base(
            result,
            true,
            availableFilters,
            "Search courses"
        )
        {
            Courses = result.ItemsToDisplay.Select(c => new SearchableDelegateCourseStatisticsViewModel(c));
        }

        public IEnumerable<SearchableDelegateCourseStatisticsViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.Completed,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
