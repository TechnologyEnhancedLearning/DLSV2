namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.User;

    public class GroupDelegatesRemoveViewModel
    {
        public GroupDelegatesRemoveViewModel() { }

        public GroupDelegatesRemoveViewModel(DelegateUser delegateUser, string groupName, int groupId)
        {
            GroupId = groupId;
            GroupName = groupName;
            DelegateName = (string.IsNullOrEmpty(delegateUser.FirstName) ? "" : $"{delegateUser.FirstName} ") + delegateUser.LastName;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string DelegateName { get; set; }

        public bool Confirm { get; set; }

        public bool RemoveProgress { get; set; }
    }
}
