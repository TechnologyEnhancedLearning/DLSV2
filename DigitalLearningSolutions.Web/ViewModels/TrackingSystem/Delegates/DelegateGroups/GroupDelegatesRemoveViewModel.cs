namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;

    public class GroupDelegatesRemoveViewModel
    {
        public GroupDelegatesRemoveViewModel() { }

        public GroupDelegatesRemoveViewModel(GroupDelegate delegateUser, string groupName, int groupId)
        {
            GroupId = groupId;
            GroupName = groupName;
            DelegateName = DisplayStringHelper.GetDelegateNameString(delegateUser.FirstName, delegateUser.LastName);
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string DelegateName { get; set; }

        public bool ConfirmRemovalFromGroup { get; set; }

        public bool RemoveProgress { get; set; }
    }
}
