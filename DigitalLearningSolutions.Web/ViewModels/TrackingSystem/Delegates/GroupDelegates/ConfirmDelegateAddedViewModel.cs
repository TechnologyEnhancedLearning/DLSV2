namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    public class ConfirmDelegateAddedViewModel
    {
        public ConfirmDelegateAddedViewModel() { }

        public ConfirmDelegateAddedViewModel(string delegateName, string groupName, int groupId)
        {
            DelegateName = delegateName;
            GroupName = groupName;
            GroupId = groupId;
        }

        public string DelegateName { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
    }
}
