namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;

    public class RemoveGroupDelegateViewModel
    {
        public RemoveGroupDelegateViewModel() { }

        public RemoveGroupDelegateViewModel(GroupDelegate delegateUser, string groupName, int groupId, int? progressId)
        {
            GroupId = groupId;
            GroupName = groupName;
            DelegateName = DisplayStringHelper.GetDelegateNameString(delegateUser.FirstName, delegateUser.LastName);
            RemoveStartedEnrolmentsEnabled = progressId.HasValue;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string DelegateName { get; set; }

        [BooleanMustBeTrue(ErrorMessage = "You must confirm before removing this user from the group")]
        public bool ConfirmRemovalFromGroup { get; set; }

        public bool RemoveStartedEnrolments { get; set; }

        public bool RemoveStartedEnrolmentsEnabled { get; set; }
    }
}
