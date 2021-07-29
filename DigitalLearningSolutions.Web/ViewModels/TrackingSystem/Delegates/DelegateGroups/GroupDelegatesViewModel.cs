namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class GroupDelegatesViewModel : BaseSearchablePageViewModel
    {
        public GroupDelegatesViewModel(
            int groupId,
            string groupName,
            IEnumerable<GroupDelegate> groupDelegates,
            int page
        ) : base(null, DefaultSortByOptions.Name.PropertyName, Ascending, page)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupName, DelegateGroupPage.Delegates);

            var sortedItems = GenericSortingHelper.SortAllItems(
                groupDelegates.AsQueryable(),
                DefaultSortByOptions.Name.PropertyName,
                Ascending
            ).ToList();
            
            MatchingSearchResults = sortedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedItems);
            GroupDelegates = paginatedItems.Select(groupDelegate => new GroupDelegateViewModel(groupDelegate));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupDelegateViewModel> GroupDelegates { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };
    }
}
