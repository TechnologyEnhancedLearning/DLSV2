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
        ) : base(null, page, false)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Delegates);

            var sortedItems = GenericSortingHelper.SortAllItems(
                groupDelegates.AsQueryable(),
                DefaultSortByOptions.Name.PropertyName,
                Ascending
            ).ToList();
            
            MatchingSearchResults = sortedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedItems);
            GroupDelegates = paginatedItems.Select(groupDelegate => new GroupDelegateExpandableViewModel(groupDelegate));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupDelegateExpandableViewModel> GroupDelegates { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };
    }
}
