namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class GroupDelegatesViewModel : BasePaginatedViewModel<GroupDelegate>
    {
        public GroupDelegatesViewModel(
            int groupId,
            string groupName,
            PaginateResult<GroupDelegate> result
        ) : base(result)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Delegates);
            GroupDelegates = result.ItemsToDisplay.Select(groupDelegate => new GroupDelegateViewModel(groupDelegate));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupDelegateViewModel> GroupDelegates { get; }
    }
}
