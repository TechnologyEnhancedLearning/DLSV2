namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AddDelegateToGroup
{
    public class DelegateAddedToGroupConfirmationViewModel
    {
        public DelegateAddedToGroupConfirmationViewModel() { }

        public DelegateAddedToGroupConfirmationViewModel(string delegateName, string groupName, int groupId)
        {
            DelegateName = delegateName;
            GroupName = groupName;
            GroupId = groupId;
        }

        public string? DelegateName { get; set; }
        public string? GroupName { get; set; }
        public int GroupId { get; set; }
    }
}
