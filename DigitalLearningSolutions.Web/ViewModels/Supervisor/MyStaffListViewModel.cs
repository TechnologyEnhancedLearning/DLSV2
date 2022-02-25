namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class MyStaffListViewModel : BaseSearchablePageViewModel
    {
        public MyStaffListViewModel(
            IEnumerable<SupervisorDelegateDetailViewModel> supervisorDelegateDetailViewModels,
            CentreRegistrationPrompts centreRegistrationPrompts,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search administrators")
        {
            CentreRegistrationPrompts = centreRegistrationPrompts;
            var sortedItems = GenericSortingHelper.SortAllItems(
                supervisorDelegateDetailViewModels.AsQueryable(),
                sortBy,
                sortDirection
            );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);
            SuperviseDelegateDetailViewModels = paginatedItems;
        }

        public IEnumerable<SupervisorDelegateDetailViewModel> SuperviseDelegateDetailViewModels { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name,
        };

        public CentreRegistrationPrompts CentreRegistrationPrompts { get; set; }

        public override bool NoDataFound => !SuperviseDelegateDetailViewModels.Any() && NoSearchOrFilter;
    }
}
