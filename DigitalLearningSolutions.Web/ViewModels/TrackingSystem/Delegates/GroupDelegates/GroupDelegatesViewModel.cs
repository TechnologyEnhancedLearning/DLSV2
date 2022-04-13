namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class GroupDelegatesViewModel : BasePaginatedViewModel<GroupDelegate>
    {
        public GroupDelegatesViewModel(
            int groupId,
            string groupName,
            PaginationResult<GroupDelegate> result
        ) : base(result)
        {
            GroupId = groupId;
            GroupName = groupName;
            var routeData = new Dictionary<string, string> { { "groupId", groupId.ToString() } };
            TabsNavLinks = new TabsNavViewModel(DelegateGroupTab.Delegates, routeData);
            GroupDelegates = result.ItemsToDisplay.Select(
                groupDelegate => new GroupDelegateViewModel(
                    groupDelegate,
                    result.GetReturnPageQuery($"{groupDelegate.DelegateId}-card")
                )
            );
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public TabsNavViewModel TabsNavLinks { get; set; }

        public IEnumerable<GroupDelegateViewModel> GroupDelegates { get; }
    }
}
