namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class DelegateGroupsViewModel : BaseSearchablePageViewModel<Group>
    {
        public DelegateGroupsViewModel(
            SearchSortFilterPaginationResult<Group> result,
            IEnumerable<FilterModel> availableFilters
        ) : base(result, true, availableFilters)
        {
            var returnPage = string.IsNullOrWhiteSpace(SearchString) ? Page : 1;
            DelegateGroups = result.ItemsToDisplay.Select(g =>
                {
                    var cardId = $"{g.GroupId}-card";
                    return new SearchableDelegateGroupViewModel(g, result.GetReturnPageQuery(cardId));
                }
            );
        }

        public IEnumerable<SearchableDelegateGroupViewModel> DelegateGroups { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateGroupsSortByOptions.Name,
            DelegateGroupsSortByOptions.NumberOfDelegates,
            DelegateGroupsSortByOptions.NumberOfCourses,
        };

        public override bool NoDataFound => !DelegateGroups.Any() && NoSearchOrFilter;
    }
}
