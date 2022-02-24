namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CourseSetupViewModel : BaseSearchablePageViewModel
    {
        public CourseSetupViewModel(
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
            var searchedItems = GenericSearchHelper.SearchItems(courses.AsQueryable(), SearchString).ToList();
            var paginatedItems = SortFilterAndPaginate(searchedItems);

            Courses = paginatedItems.Select(c => new SearchableCourseStatisticsViewModel(c));

            Filters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics);
        }

        public IEnumerable<SearchableCourseStatisticsViewModel> Courses { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseSortByOptions.CourseName,
            CourseSortByOptions.TotalDelegates,
            CourseSortByOptions.InProgress,
        };

        public override bool NoDataFound => !Courses.Any() && NoSearchOrFilter;
    }
}
