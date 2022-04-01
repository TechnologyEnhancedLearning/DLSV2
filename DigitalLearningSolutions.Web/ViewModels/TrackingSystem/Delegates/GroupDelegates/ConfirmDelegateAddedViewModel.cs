namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;

    public class ConfirmDelegateAddedViewModel
    {
        public ConfirmDelegateAddedViewModel() { }

        public ConfirmDelegateAddedViewModel(DelegateUser delegateUser, string groupName, int groupId)
        {
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                delegateUser.FirstName,
                delegateUser.LastName
            );
            GroupName = groupName;
            GroupId = groupId;
        }

        public string DelegateName { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
    }
}
