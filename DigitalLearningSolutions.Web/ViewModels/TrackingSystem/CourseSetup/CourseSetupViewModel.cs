namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CourseSetupViewModel : BaseSearchablePageViewModel
    {
        public CourseSetupViewModel(
            IEnumerable<CourseStatistics> courses,
            IEnumerable<string> categories,
            IEnumerable<string> topics,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page,
            int? itemsPerPage
        ) : base(searchString, page, true, sortBy, sortDirection, filterBy, itemsPerPage ?? DefaultItemsPerPage, "Search courses")
        {
            var sortedItems = GenericSortingHelper.SortAllItems(
                courses.AsQueryable(),
                sortBy,
                sortDirection
            );

            var filteredItems = FilteringHelper.FilterItems(sortedItems.AsQueryable(), filterBy).ToList();
            var searchedItems = GenericSearchHelper.SearchItems(filteredItems, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);

            Courses = paginatedItems.Select(c => new SearchableCourseStatisticsViewModel(c));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics);
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.TotalDelegates,
            CourseSortByOptions.InProgress
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
