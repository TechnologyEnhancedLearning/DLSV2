namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;

    public class RemoveGroupDelegatesViewModel
    {
        public RemoveGroupDelegatesViewModel() { }

        public RemoveGroupDelegatesViewModel(GroupDelegate delegateUser, string groupName, int groupId, int? progressId)
        {
            GroupId = groupId;
            GroupName = groupName;
            DelegateName = DisplayStringHelper.GetDelegateNameString(delegateUser.FirstName, delegateUser.LastName);
            RemoveStartedEnrolmentsEnabled = progressId.HasValue;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string DelegateName { get; set; }

        public bool ConfirmRemovalFromGroup { get; set; }

        public bool RemoveStartedEnrolments { get; set; }

        public bool RemoveStartedEnrolmentsEnabled { get; set; }
    }
}
