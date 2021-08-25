﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

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

            var sortedItems = groupDelegates.OrderBy(gd => gd.Name).ToList();

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
