namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class GroupDelegatesViewModel : BasePaginatedViewModel
    {
        public GroupDelegatesViewModel(
            int groupId,
            string groupName,
            IEnumerable<GroupDelegate> groupDelegates,
            int page
        ) : base(page)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Delegates);

            var sortedItems = GenericSortingHelper.SortAllItems(
                groupDelegates.AsQueryable(),
                nameof(GroupDelegate.SearchableName),
                GenericSortingHelper.Ascending
            ).ToList();

            MatchingSearchResults = sortedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedItems);
            GroupDelegates =
                paginatedItems.Select(groupDelegate => new GroupDelegateViewModel(groupDelegate));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupDelegateViewModel> GroupDelegates { get; }
    }
}
