namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class DelegateCoursesViewModel : BaseSearchablePageViewModel
    {
        public DelegateCoursesViewModel(
            IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page,
            int? itemsPerPage
        ) : base(
            searchString,
            page,
            true,
            sortBy,
            sortDirection,
            filterBy,
            itemsPerPage ?? DefaultItemsPerPage,
            "Search courses"
        )
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                courses.AsQueryable(),
                sortBy,
                sortDirection
            );

            var filteredItems = FilteringHelper.FilterItems(sortedItems.AsQueryable(), filterBy).ToList();
            var searchedItems = GenericSearchHelper.SearchItems(filteredItems, SearchString).ToList();
            var paginatedItems = SortFilterAndPaginate(searchedItems);

            Courses = paginatedItems.Select(c => new SearchableDelegateCourseStatisticsViewModel(c));
            Filters = DelegateCourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics);
        }

        public IEnumerable<SearchableDelegateCourseStatisticsViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.TotalDelegates,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
