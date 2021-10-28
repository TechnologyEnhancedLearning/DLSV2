namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SelectedCourseDetails : BaseSearchablePageViewModel
    {
        public SelectedCourseDetails(
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

            var sortedItems = GenericSortingHelper.SortAllItems(
                courseDelegatesData.Delegates.AsQueryable(),
                sortBy,
                sortDirection
            );
            var filteredItems = FilteringHelper.FilterItems(sortedItems.AsQueryable(), filterBy).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(filteredItems);
            Delegates = paginatedItems.Select(d => new SearchableCourseDelegateViewModel(d));
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels();
        }

        public bool Active { get; set; }
        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            CourseDelegatesSortByOptions.Name,
            CourseDelegatesSortByOptions.LastUpdatedDate,
            CourseDelegatesSortByOptions.EnrolledDate,
            CourseDelegatesSortByOptions.CompleteByDate,
            CourseDelegatesSortByOptions.CompletedDate,
            CourseDelegatesSortByOptions.PassRate,
        };

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
