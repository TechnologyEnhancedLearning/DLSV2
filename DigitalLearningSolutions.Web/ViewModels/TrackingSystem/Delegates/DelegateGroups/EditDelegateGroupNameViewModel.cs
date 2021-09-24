namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class EditDelegateGroupNameViewModel
    {
        public int GroupId { get; set; }
        public string? GroupLabel { get; set; }

        public EditDelegateGroupNameViewModel() { }

        public EditDelegateGroupNameViewModel(Group group)
        {
            GroupId = group.GroupId;
            GroupLabel = group.GroupLabel;
        }
    }
}
