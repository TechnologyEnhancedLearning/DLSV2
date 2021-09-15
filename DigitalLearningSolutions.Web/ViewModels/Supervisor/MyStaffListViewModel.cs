namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using System.Collections.Generic;
    public class MyStaffListViewModel : BaseSearchablePageViewModel
    {
        public MyStaffListViewModel(
            IEnumerable<SupervisorDelegateDetail> supervisorDelegateDetails,
            CentreCustomPrompts centreCustomPrompts,
            string? searchString,
            string sortBy,
            string sortDirection,
            int page
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search administrators")
        {
            CentreCustomPrompts = centreCustomPrompts;
            var sortedItems = GenericSortingHelper.SortAllItems(
           supervisorDelegateDetails.AsQueryable(),
           sortBy,
           sortDirection
       );
            var searchedItems = GenericSearchHelper.SearchItems(sortedItems, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);
            SuperviseDelegateDetails = paginatedItems;

        }
        public IEnumerable<SupervisorDelegateDetail> SuperviseDelegateDetails { get; set; }
        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
       {
            DefaultSortByOptions.Name
        };
        public CentreCustomPrompts CentreCustomPrompts { get; set; }

        public override bool NoDataFound => !SuperviseDelegateDetails.Any() && NoSearchOrFilter;
    }
}
