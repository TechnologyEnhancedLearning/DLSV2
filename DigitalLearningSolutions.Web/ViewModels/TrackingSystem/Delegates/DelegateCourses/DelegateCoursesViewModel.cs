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
            CentreCourseDetails details,
            string? searchString,
            string sortBy,
            string sortDirection,
            string? existingFilterString,
            int page,
            int? itemsPerPage
        ) : base(
            searchString,
            page,
            true,
            sortBy,
            sortDirection,
            existingFilterString,
            itemsPerPage ?? DefaultItemsPerPage,
            "Search courses"
        )
        {
            var searchedItems = GenericSearchHelper.SearchItems(details.Courses.AsQueryable(), SearchString).ToList();
            var paginatedItems = SortFilterAndPaginate(searchedItems);

            Courses = paginatedItems.Select(c => new SearchableDelegateCourseStatisticsViewModel(c));
            Filters = DelegateCourseStatisticsViewModelFilterOptions.GetFilterOptions(details.Categories, details.Topics);
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
